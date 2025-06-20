using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Holds board cell data and line-clear logic.
/// Attach to a `GridManager` GameObject under Canvas.
/// </summary>
public class GridManager : MonoBehaviour
{
    public const int Width = 10;  // 列数
    public const int Height = 16; // 行数

    /// <summary>累計開放セル数</summary>
    public int OpenedBackgroundCells { get; private set; }

    // 0 = empty, 1 = occupied
    private int[,] _grid = new int[Width, Height];

    // 背景セルとブロックオブジェクト
    private BackgroundCell[,] _bg = new BackgroundCell[Width, Height];
    private Block[,] _blockObjs;

    // フレーム遅延で子オブジェクト掃除
    private bool cleanupPending = false;

    [Header("セルの１辺ピクセル数 (0以下で自動)")]
    [Tooltip("値を入れると 1セル＝overrideCellSize px になります")]
    public float overrideCellSize = 0.5f;
    private float _cellSize;
    public float CellSize => _cellSize; // 外部参照用

    private HashSet<Vector2Int> _unlockedCells = new HashSet<Vector2Int>();

    void Awake()
    {
        // グリッド配列初期化
        _grid = new int[Width, Height];
        _blockObjs = new Block[Width, Height];

        // セルサイズ決定
        var rt = GetComponent<RectTransform>();
        if (overrideCellSize > 0f)
        {
            _cellSize = overrideCellSize;
        }
        else
        {
            float w = rt.rect.width / Width;
            float h = rt.rect.height / Height;
            _cellSize = Mathf.Min(w, h);
        }

        float gridW = _cellSize * Width;
        float gridH = _cellSize * Height;

        // 自身を画面中央に配置
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(gridW, gridH);
        rt.anchoredPosition = Vector2.zero;

        // 背景画像
        var bgGo = new GameObject("BG_Image",
                                  typeof(RectTransform),
                                  typeof(CanvasRenderer),
                                  typeof(Image));
        bgGo.transform.SetParent(transform, false);
        bgGo.transform.SetAsFirstSibling();
        var bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = new Vector2(0.5f, 0.5f);
        bgRt.anchorMax = new Vector2(0.5f, 0.5f);
        bgRt.pivot = new Vector2(0.5f, 0.5f);
        bgRt.sizeDelta = new Vector2(gridW, gridH);
        bgRt.anchoredPosition = Vector2.zero;
        var bgImg = bgGo.GetComponent<Image>();
        bgImg.type = Image.Type.Simple;
        bgImg.preserveAspect = false;
        bgImg.raycastTarget = false;

        // マスクタイル生成
        float halfW = gridW * 0.5f;
        float halfH = gridH * 0.5f;
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var tileGo = new GameObject($"Mask_{x}_{y}",
                                            typeof(RectTransform),
                                            typeof(CanvasRenderer),
                                            typeof(Image));
                tileGo.transform.SetParent(transform, false);
                var tileRt = tileGo.GetComponent<RectTransform>();
                tileRt.anchorMin = new Vector2(0.5f, 0.5f);
                tileRt.anchorMax = new Vector2(0.5f, 0.5f);
                tileRt.pivot = new Vector2(0.5f, 0.5f);
                tileRt.sizeDelta = Vector2.one * _cellSize;
                tileRt.anchoredPosition = new Vector2(
                    x * _cellSize - halfW + _cellSize * 0.5f,
                    y * _cellSize - halfH + _cellSize * 0.5f
                );
                var img = tileGo.GetComponent<Image>();
                img.color = new Color(0, 0, 0, 0.5f);
                img.raycastTarget = false;
                var cell = tileGo.AddComponent<BackgroundCell>();
                cell.Lock();
                _bg[x, y] = cell;
            }
    }

    /// <summary>論理グリッドに直接値を設定</summary>
    public void SetCell(int x, int y, int value)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return;
        _grid[x, y] = value;
    }

    /// <summary>初期ブロックを UI に生成</summary>
    public void SpawnInitialBlocks(Vector2Int[] initialCells, Sprite blockSprite)
    {
        foreach (var cell in initialCells)
        {
            int x = cell.x, y = cell.y;
            if (x < 0 || x >= Width || y < 0 || y >= Height) continue;
            _grid[x, y] = 1;
            var go = new GameObject($"InitBlock_{x}_{y}",
                                    typeof(RectTransform),
                                    typeof(CanvasRenderer),
                                    typeof(Image));
            go.transform.SetParent(transform, false);
            var blk = go.AddComponent<Block>();
            blk.Init(x, y, _cellSize, blockSprite);
            _blockObjs[x, y] = blk;
        }
    }

    /// <summary>置けるかどうかを判定</summary>
    public bool CanPlace(Vector2Int[] cells, Vector2Int origin)
    {
        foreach (var c in cells)
        {
            int x = origin.x + c.x;
            int y = origin.y + c.y;
            if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
            if (_grid[x, y] != 0) return false;
        }
        return true;
    }

    /// <summary>ピースを配置し、行消去＋アンロック数を返却</summary>
    public int Place(PieceData data, Vector2Int origin)
    {
        // 論理グリッド
        foreach (var c in data.cells)
        {
            int x = origin.x + c.x;
            int y = origin.y + c.y;
            if (x < 0 || x >= Width || y < 0 || y >= Height) continue;
            _grid[x, y] = 1;
        }
        // 見た目ブロック
        foreach (var c in data.cells)
        {
            int x = origin.x + c.x;
            int y = origin.y + c.y;
            if (x < 0 || x >= Width || y < 0 || y >= Height) continue;
            var go = new GameObject("Block",
                                    typeof(RectTransform),
                                    typeof(CanvasRenderer),
                                    typeof(Image));
            go.transform.SetParent(transform, false);
            var blk = go.AddComponent<Block>();
            blk.Init(x, y, _cellSize, data.blockSprite);
            _blockObjs[x, y] = blk;
        }
        return ClearFullLines();
    }

    /// <summary>マスクを全セルロック</summary>
    public void LockAllMasks()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                _bg[x, y].Lock();
                if (!_bg[x, y].gameObject.activeSelf)
                    _bg[x, y].gameObject.SetActive(true);
            }
        OpenedBackgroundCells = 0;
    }

    /// <summary>すべてのブロックを非表示</summary>
    public void HideAllBlocks()
    {
        foreach (var blk in _blockObjs)
            if (blk != null) blk.gameObject.SetActive(false);
    }

    /// <summary>すべてのブロックを再表示</summary>
    public void ShowAllBlocks()
    {
        foreach (var blk in _blockObjs)
            if (blk != null) blk.gameObject.SetActive(true);
    }

    /// <summary>すべてのマスクを非表示</summary>
    public void HideAllMasks()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                _bg[x, y].gameObject.SetActive(false);
    }

    /// <summary>
    /// num 個だけランダムにセルをアンロックし、
    /// そこに残っているビジュアルなミノも消す
    /// </summary>
    public int UnlockRandomCells(int num)
    {
        // ─── 候補を集める ─────────────────────
        var candidates = new List<Vector2Int>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (_bg[x, y].IsLocked)
                    candidates.Add(new Vector2Int(x, y));
            }
        }

        int unlocked = 0;
        // ─── ランダムに n セル分ループ ────────────────
        for (int i = 0; i < num && candidates.Count > 0; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            var c = candidates[idx];
            candidates.RemoveAt(idx);

            // ─── 1) ビジュアルなミノ (Block) が残っていたら消す ───
            var blk = _blockObjs[c.x, c.y];
            if (blk != null)
            {
                Destroy(blk.gameObject);
                _blockObjs[c.x, c.y] = null;
            }

            // ─── 2) ロジックグリッドにフラグ立て
            _grid[c.x, c.y] = 0;

            // ─── 3) マスク解除 & カウンター用に数をカウントアップ
            _bg[c.x, c.y].Unlock();
            unlocked++;
        }

        // ─── 4) もしフレーム遅延で余分な空 Piece オブジェクトを掃除しているなら…
        cleanupPending = true;

        return unlocked;
    }

    /// <summary>ライン消去してアンロック数を返</summary>
    private int ClearFullLines()
    {
        bool[] fullRow = new bool[Height];
        bool[] fullCol = new bool[Width];
        for (int y = 0; y < Height; y++)
        {
            bool all = true; foreach (var x in Enumerable.Range(0, Width)) if (_grid[x, y] == 0) { all = false; break; }
            fullRow[y] = all;
        }
        for (int x = 0; x < Width; x++)
        {
            bool all = true; foreach (var y in Enumerable.Range(0, Height)) if (_grid[x, y] == 0) { all = false; break; }
            fullCol[x] = all;
        }
        var toClear = new List<Vector2Int>();
        for (int y = 0; y < Height; y++) if (fullRow[y]) for (int x = 0; x < Width; x++) toClear.Add(new Vector2Int(x, y));
        for (int x = 0; x < Width; x++) if (fullCol[x]) for (int y = 0; y < Height; y++) toClear.Add(new Vector2Int(x, y));
        toClear = toClear.Distinct().ToList();
        int unlocked = 0;
        foreach (var c in toClear)
        {
            if (_blockObjs[c.x, c.y] != null) { Destroy(_blockObjs[c.x, c.y].gameObject); _blockObjs[c.x, c.y] = null; }
            _grid[c.x, c.y] = 0;
            if (_bg[c.x, c.y].IsLocked) { _bg[c.x, c.y].Unlock(); unlocked++; }
        }
        cleanupPending = true;
        return unlocked;
    }

    void LateUpdate()
    {
        if (!cleanupPending) return;
        cleanupPending = false;
        // 空 Piece を掃除
        foreach (var piece in GetComponentsInChildren<Piece>())
            if (piece.transform.childCount == 0)
                Destroy(piece.gameObject);
    }

    /// <summary>リセット処理</summary>
    public void ResetGrid()
    {
        _grid = new int[Width, Height];
        OpenedBackgroundCells = 0;
        cleanupPending = false;
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                _bg[x, y].Lock();
    }

    public List<Vector2Int> PreviewRandomUnlocks(int n)
    {
        var candidates = new List<Vector2Int>();
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (_bg[x, y].IsLocked)
                    candidates.Add(new Vector2Int(x, y));

        var picked = new List<Vector2Int>();
        for (int i = 0; i < n && candidates.Count > 0; i++)
        {
            int idx = Random.Range(0, candidates.Count);
            var c = candidates[idx];
            candidates.RemoveAt(idx);

            _bg[c.x, c.y].PreviewUnlock();
            picked.Add(c);
        }
        return picked;
    }

    // プレビューで外したマスクを「元に戻す」だけの呼び出し
    public void ClearPreviewUnlocks(IEnumerable<Vector2Int> cells)
    {
        foreach (var c in cells)
            _bg[c.x, c.y].ClearPreview();
    }

    public void CommitUnlocks(IEnumerable<Vector2Int> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.x < 0 || cell.x >= Width || cell.y < 0 || cell.y >= Height)
                continue;

            var target = _bg[cell.x, cell.y];
            if (target == null)
                continue;

            if (target.IsLocked)
            {
                // 見た目上アンロック
                target.Unlock();
                // 論理的にも解放済みセルとして記録（2度目以降除外）
                _unlockedCells.Add(cell);

                // ブロックが残っていれば削除
                var blk = _blockObjs[cell.x, cell.y];
                if (blk != null)
                {
                    Destroy(blk.gameObject);
                    _blockObjs[cell.x, cell.y] = null;
                }

                // グリッド上の値も 0 に（空セル扱いに）
                _grid[cell.x, cell.y] = 0;
            }
        }
    }
}
