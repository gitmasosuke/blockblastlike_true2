using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // ← 追加（Inputだけでよければ不要）

public class SlotEffectController : MonoBehaviour
{
    [Header("参照設定")]
    public GridManager gridManager; // GridManager をここにアサイン
    public Button slotButton;       // 演出を開始するボタンをアサイン

    [Header("スロット設定")]
    [Tooltip("1ループあたりにプレビュー解除するマス数")]
    public int unlockCount = 10;

    [Tooltip("プレビューを切り替える間隔(秒)")]
    public float cycleInterval = 0.1f;

    [Tooltip("演出全体の再生時間(秒)")]
    public float totalDuration = 2f;

    [Header("ポップアップ設定")]
    public GameObject slotPopupPrefab;   // 上で作った SlotPopupPanel.prefab
    private GameObject _popupInstance;
    private Button _btnClose;
    private Button _btnWatchAd;

    Coroutine _slotRoutine;
    List<Vector2Int> _lastPreview;

    List<Vector2Int> currentPreview = new();

    public GameObject tapToNextText;  // “Tap To Next” テキスト（Hierarchy の GameObject）

    private bool firstSlot = true;

    void Awake()
    {
        // Inspector に設定がなければシーンから自動検索
        if (gridManager == null)
        {
            // Unity 2023.1 以降なら
            gridManager = Object.FindFirstObjectByType<GridManager>();
            // もしそれでも null なら警告
            if (gridManager == null)
                Debug.LogError("SlotEffectController: GridManager が見つかりません！");
        }

        if (slotButton != null)
            slotButton.onClick.AddListener(StartSlotEffect);
    }

    /// <summary>
    /// ボタン押下時に呼ばれる。既存の演出を止して再スタート。
    /// </summary>
    public void StartSlotEffect()
    {
        if (_slotRoutine != null)
            StopCoroutine(_slotRoutine);

        // “Tap To Next” テキストを表示
        tapToNextText.SetActive(true);

        // 直前のプレビューが残っていればクリア
        if (_lastPreview != null && _lastPreview.Count > 0)
            gridManager.ClearPreviewUnlocks(_lastPreview);

        _slotRoutine = StartCoroutine(SlotRoutine());
    }

    IEnumerator SlotRoutine()
    {
        float elapsed = 0f;
        List<Vector2Int> preview = new List<Vector2Int>();

        while (elapsed < totalDuration)
        {

            // ── ① 前回プレビューを必ずクリア
            if (preview.Count > 0)
            {
                gridManager.ClearPreviewUnlocks(preview);
                preview.Clear();
            }

            // ── ② 新しいプレビュー取得＆表示
            preview = gridManager.PreviewRandomUnlocks(unlockCount);
            currentPreview = new List<Vector2Int>(preview); //ここを追加

            // ── 演出間隔待機
            yield return new WaitForSeconds(cycleInterval);

            // ③ cycleInterval秒待機
            float waited = 0f;
            while (waited < cycleInterval)
            {
                // ここで「画面タップを検出したら早期抜け」もOK
                if (Input.GetMouseButtonDown(0))
                {
                    // 演出終了時の最後のプレビューもそのまま残しておく
                    _lastPreview = new List<Vector2Int>(currentPreview);

                    StopSlot();

                    // ここで演出は止まった状態なので、ポップアップを出す
                    if(firstSlot)
                        ShowPopup();


                    yield break;
                }
                waited += Time.deltaTime;
                yield return null;
            }

            elapsed += cycleInterval;
        }

        // 演出終了時も念のためクリア
        if (preview.Count > 0)
        {
            gridManager.ClearPreviewUnlocks(preview);
            preview.Clear();
        }
        
        // スロット終了時にプレビューはそのまま残しておく（確定待ち）
        _slotRoutine = null;


    }

    public void StopSlot()
    {
        if (_slotRoutine != null)
        {
            StopCoroutine(_slotRoutine);
            _slotRoutine = null;

            // ① プレビュー解除
            if (_lastPreview.Count > 0)
                gridManager.ClearPreviewUnlocks(_lastPreview);

            // 最後のプレビュー状態を正式に解放
            gridManager.CommitUnlocks(currentPreview);

            //カウントを GameManager に通知
            GameManager.Instance.AddUnlockedCellCount(currentPreview.Count);

            // プレビューリストをリセット
            currentPreview.Clear();

            // “Tap To Next” テキストを隠す
            tapToNextText.SetActive(false);

            _lastPreview.Clear();
        }
    }

    /// <summary>演出終了後にポップアップを表示</summary>
    public void ShowPopup()
    {

        //初回フラグクリア
        firstSlot = false;

        if (slotPopupPrefab == null)
        {
            Debug.LogError("slotPopupPrefab がセットされていません！");
            return;
        }

        // インスタンス化
        _popupInstance = Instantiate(
            slotPopupPrefab,
            gridManager.transform.parent  // Canvas の直下に置く
        );

        // ③ RectTransform を中央に配置
        var rt = _popupInstance.GetComponent<RectTransform>();
        if (rt != null)
        {
            // アンカー／ピボットを中心に
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            // ローカル座標(0,0)がキャンバス中心になる
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;

            // 最前面に
            _popupInstance.transform.SetAsLastSibling();
        }

        // ボタン取得
        _btnClose = _popupInstance.transform.Find("ButtonClose").GetComponent<Button>();
        _btnWatchAd = _popupInstance.transform.Find("ButtonWatchAd").GetComponent<Button>();

        // リスナー登録
        _btnClose.onClick.AddListener(OnPopupClose);
        _btnWatchAd.onClick.AddListener(OnPopupWatchAd);
    }

    /// <summary>「閉じる」押下時：そのまま最終解放を本番処理して終了</summary>
    private void OnPopupClose()
    {
        CommitLastPreviewAndCleanup();
        Destroy(_popupInstance);
    }

    /// <summary>「CM視聴」押下時：再度ルーレット演出 → 最終解放 → 終了</summary>
    private void OnPopupWatchAd()
    {
        // 一度ポップアップを閉じる
        Destroy(_popupInstance);

        // 再度ルーレット演出をスタート
        StartSlotEffect();

        // ※もし実際に広告を流すならここで広告再生を挟んでから StartSlotEffect() を呼んでください
    }

    /// <summary>
    /// 最後にプレビューされていたセルを本番アンロックして後片付け
    /// </summary>
    private void CommitLastPreviewAndCleanup()
    {
        if (_lastPreview != null && _lastPreview.Count > 0)
        {
            gridManager.CommitUnlocks(_lastPreview);
            _lastPreview.Clear();
        }
    }
}