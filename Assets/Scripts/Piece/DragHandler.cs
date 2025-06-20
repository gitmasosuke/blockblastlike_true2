using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour,
                           IBeginDragHandler,
                           IDragHandler,
                           IEndDragHandler
{
    RectTransform _rect;
    Canvas _canvas;
    PieceUI _pieceUI;
    BoardHighlighter _highlighter;
    Vector3 _originalScale;

    // 追加：
    private Transform _originalParent;
    private Vector2 _originalAnchoredPos;

    // 追加：直前にハイライトしていたセル
    private Vector2Int _lastHighlightOrigin;
    private bool _hasLastHighlight = false;
    // ドラッグ開始位置からピース左下までのオフセット
    private Vector2 _dragOffset;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _pieceUI = GetComponent<PieceUI>();

        if (GameManager.Instance != null &&
            GameManager.Instance.gridManager != null)
        {
            _highlighter = GameManager.Instance.gridManager
                                        .GetComponent<BoardHighlighter>();
        }
        else
        {
            Debug.LogError("[DragHandler] GameManager or GridManager is missing");
        }
        // 手札時の元の拡大率を保存
        _originalScale = transform.localScale;
    }

    // ドラッグ開始時
    public void OnBeginDrag(PointerEventData eventData)
    {
        // GameManager が取得できなければ処理しない
        if (GameManager.Instance == null ||
            GameManager.Instance.gridManager == null)
        {
            Debug.LogError("[DragHandler] Cannot start drag - GameManager missing");
            return;
        }

        // 親と位置を覚えておく
        _originalParent = transform.parent;
        _originalAnchoredPos = _rect.anchoredPosition;

        // ルート Canvas にぶら下げてスクリーン座標で移動できるように
        transform.SetParent(_canvas.transform, true);
        transform.SetAsLastSibling();

        // レイアウトグループから外れると Anchor が (0,1) に残るので中央に戻す
        _rect.anchorMin = _rect.anchorMax = new Vector2(0.5f, 0.5f);
        _rect.anchoredPosition = Vector2.zero;

        // グリッド上の「1セル辺長」を取得
        float gridSize = GameManager.Instance.gridManager.CellSize;

        if (_pieceUI == null)
            _pieceUI = GetComponent<PieceUI>();
        if (_pieceUI == null)
        {
            Debug.LogError("[DragHandler] PieceUI component missing");
            return;
        }

        // 手札表示時に PieceUI が使っている1セル辺長
        float handSize = _pieceUI.CellSizeInHand;

        // 左下までのオフセットを計算
        //   PieceUI の RectTransform はピース実サイズより大きいため
        //   実際の Rect サイズを考慮してスクリーン座標を求める

        // Rect の左下ワールド座標を取得（拡大前）
        Vector3 worldBottomLeft = _rect.TransformPoint(new Vector3(
            -_rect.pivot.x * _rect.rect.width,
            -_rect.pivot.y * _rect.rect.height,
            0f));
        Vector2 bottomLeftScreen = RectTransformUtility.WorldToScreenPoint(
            _canvas.worldCamera, worldBottomLeft);

        // ポインタと左下の距離（拡大前）
        Vector2 pointerOffset = eventData.position - bottomLeftScreen;

        // ドラッグ中はこの倍率で全体を拡大
        float scale = gridSize / handSize;
        transform.localScale = _originalScale * scale;

        // 拡大後も同じセルがポインタ下に来るようオフセットを更新
        _dragOffset = pointerOffset * scale;

        // 最初のハイライト表示
        UpdateHighlight(eventData);
    }

    // ドラッグ中
    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.gridManager == null)
            return;

        // まずハイライトを更新して原点セルを取得
        UpdateHighlight(eventData);

        Vector2 bottomLeftScreen = eventData.position - _dragOffset;
        Vector2Int dummyOrigin;
        float cellW, cellH;
        bool canPlace = GetGridOrigin(bottomLeftScreen, _canvas.worldCamera,
                                      out dummyOrigin, out cellW, out cellH);

        if (_hasLastHighlight && (canPlace || cellW > 0f))
        {
            var grid = GameManager.Instance.gridManager;
            var rtGrid = grid.GetComponent<RectTransform>();
            float halfW = rtGrid.rect.width * 0.5f;
            float halfH = rtGrid.rect.height * 0.5f;
            _rect.anchoredPosition = new Vector2(
                _lastHighlightOrigin.x * cellW - halfW,
                _lastHighlightOrigin.y * cellH - halfH);
        }
        else
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.GetComponent<RectTransform>(),
                bottomLeftScreen,
                _canvas.worldCamera,
                out localPoint);
            _rect.anchoredPosition = localPoint;
        }



    }

    // ドラッグ終了時

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.gridManager == null)
            return;

        // ハイライトを消す
        _highlighter.Clear();

        // グリッド上のどのセルが起点かを取得
        Vector2Int origin;
        float cellW, cellH;
        Vector2 bottomLeft = eventData.position - _dragOffset;
        bool ok = GetGridOrigin(bottomLeft, _canvas.worldCamera,
                               out origin, out cellW, out cellH);

        if (ok)
        {
            // 配置可能な位置なら
            // 全セル配置可能か（範囲外セルも含めて）チェック
            if (GameManager.Instance.gridManager
                    .CanPlace(_pieceUI.data.cells, origin))
            {
                // 置けるなら配置処理
                int unlocked = GameManager.Instance.gridManager
                                     .Place(_pieceUI.data, origin);
                GameManager.Instance.OnPiecePlaced(unlocked);
                GameManager.Instance.handManager.ReplacePiece(_pieceUI);

                // スケール／ピボットは最後に戻す
                transform.localScale = _originalScale;
                _rect.pivot = Vector2.zero; // restore bottom-left pivot
                return;
            }
        }
        else
        {
            Debug.Log("ハイライト消す処理");
            // グリッド外ならハイライトは消す
            _highlighter.Hide();
        }

        // ここまで来たら “置けない” とみなして手札に戻す
        transform.SetParent(_originalParent, true);
        _rect.anchoredPosition = _originalAnchoredPos;
        transform.localScale = _originalScale;
        _rect.pivot = Vector2.zero; // restore bottom-left pivot
    }


    void UpdateHighlight(PointerEventData eventData)
    {
        /*
        float cellW, cellH;
        Vector2Int origin;

        bool ok = GetGridOrigin(eventData, out origin, out cellW, out cellH);

        if (cellW != 0 & cellH != 0)
        {
            _highlighter.Show(_pieceUI.data.cells, origin, ok);
        }
        else
        {
            _highlighter.Clear();
        }
        */

        if (GameManager.Instance == null ||
            GameManager.Instance.gridManager == null ||
            _highlighter == null)
            return;

        var grid = GameManager.Instance.gridManager;
        var rtGrid = grid.GetComponent<RectTransform>();

        // ① ドラッグ座標がグリッド Rect 内かチェック
        if (!RectTransformUtility.RectangleContainsScreenPoint(
                rtGrid, eventData.position, _canvas.worldCamera))
        {
            _hasLastHighlight = false;     // ヒステリシスもリセット
            _highlighter.Clear();          // 完全に消す
            return;
        }

        // 以下これまでの処理…
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rtGrid, eventData.position, _canvas.worldCamera, out localPoint);
        localPoint += new Vector2(rtGrid.rect.width * rtGrid.pivot.x,
                                  rtGrid.rect.height * rtGrid.pivot.y);

        float cellW, cellH;
        Vector2Int origin;
        Vector2 bottomLeft = eventData.position - _dragOffset;
        bool ok = GetGridOrigin(bottomLeft, _canvas.worldCamera,
                                out origin, out cellW, out cellH);

        // ヒステリシス(縦方向)…
        if (_hasLastHighlight
            && origin.x == _lastHighlightOrigin.x
            && origin.y != _lastHighlightOrigin.y)
        {
            float lastCenterY = (_lastHighlightOrigin.y + 0.5f) * cellH;
            if (Mathf.Abs(localPoint.y - lastCenterY) < cellH * 0.5f)
                origin.y = _lastHighlightOrigin.y;
        }
        _lastHighlightOrigin = origin;
        _hasLastHighlight = true;

        // ⑥ 実際にハイライト表示 or クリア
        if (ok)
            _highlighter.Show(_pieceUI.data.cells, origin, ok);
        else
            _highlighter.Clear();

    }

    /// スクリーン座標 → セル原点を求め、配置可否を返す
    private bool GetGridOrigin(Vector2 bottomLeftScreen,
                               Camera cam,
                               out Vector2Int origin,
                               out float cellW,
                               out float cellH)
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.gridManager == null)
        {
            origin = Vector2Int.zero;
            cellW = cellH = 0f;
            return false;
        }

        // GridManager とその RectTransform を取得
        var grid = GameManager.Instance.gridManager;
        var rtGrid = grid.GetComponent<RectTransform>();

        // ① スクリーン座標 → ローカル座標（中心が (0,0)）
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rtGrid,
            bottomLeftScreen,
            cam,
            out localPoint
        );

        // ② 中心基準 → 左下原点に変換
        float halfW = rtGrid.rect.width * 0.5f;
        float halfH = rtGrid.rect.height * 0.5f;
        localPoint += new Vector2(halfW, halfH);

        // ここで「セル換算する前の座標」が範囲外なら NG
        if (localPoint.x < 0f ||
            localPoint.x > rtGrid.rect.width ||
            localPoint.y < 0f ||
            localPoint.y > rtGrid.rect.height)
        {
            origin = Vector2Int.zero;
            cellW = cellH = 0;
            return false;
        }

        // ③ セルサイズ (正方形) を取得
        cellW = grid.CellSize;
        cellH = cellW;

        // ④ 座標をセル単位に変換（0 から Width-1 / Height-1 の範囲内に丸め込む）
        float rawX = localPoint.x / cellW;
        float rawY = localPoint.y / cellH;


        int ox = Mathf.FloorToInt(rawX);
        int oy = Mathf.FloorToInt(rawY);
        ox = Mathf.Clamp(ox, 0, GridManager.Width - 1);
        oy = Mathf.Clamp(oy, 0, GridManager.Height - 1);

        // グリッド座標をそのまま返す
        origin = new Vector2Int(ox, oy);

        // ⑤ その位置にピースを置けるか判定して返す
        return grid.CanPlace(_pieceUI.data.cells, origin);
    }

    }
