// Assets/Scripts/Core/GameManager.cs

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("シーン内オブジェクト")] public GridManager gridManager;
    [Header("手札管理")] public HandManager handManager;
    [Header("UIカウンター")] public CounterPanel counterPanel;
    [Header("タップオーバーレイ")] public CanvasGroup tapOverlay;
    [Header("エフェクト")]
    public ParticleSystem explosionPrefab;
    [Header("ステージデータ")] public StageData stageData;


    [Header("新：背景マスターデータ")]
    public BackgroundMasterData backgroundMasterData;

    enum State { Playing, WaitingForTap, StageClear, GameOver }
    State _state;

    int _cellsUnlockedTotal;
    int _currentBgIndex;
    int _moves;

    public static GameManager Instance { get; private set; }

    [Header("ステージ選択用データ一覧")]
    public List<StageData> allStageDataList = new List<StageData>();

    void Awake()
    {

        // シングルトン初期化...
        // ここで選択ステージ番号を取得:
        int selected = PlayerPrefs.GetInt("SelectedStage", 1); // デフォルト 1
                                                               // StageData のリストを持っているならたとえば:
        stageData = allStageDataList[selected - 1];

        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }


    }

    void Start() => StartStage();

    /// <summary>ステージ開始時の一連処理</summary>
    void StartStage()
    {
        _state = State.Playing;
        _cellsUnlockedTotal = 0;
        _currentBgIndex = 0;
        _moves = 0;

        // 1) 盤面リセット
        gridManager.ResetGrid();

        // 2) 初期ブロックの論理配置
        foreach (var c in stageData.initialBlockCells)
            gridManager.SetCell(c.x, c.y, 1);

        // 3) 初期ブロックの UI 表示
        gridManager.SpawnInitialBlocks(
            stageData.initialBlockCells,
            stageData.initialBlockSprite
        );

        // 4) 背景＆マスク初期化
        UpdateBackgroundSprite();
        gridManager.LockAllMasks();


        //この処理はスロットストップボタンに割り当て
        /*
        // 5) ランダムで 1～5 マスを解放してカウント
        int initialUnlocked = gridManager.UnlockRandomCells(
            Random.Range(1, 20)
        );
        _cellsUnlockedTotal += initialUnlocked;
        */

        // 6) 手札初期化
        var piecePool = stageData.allowedPieces
                                  .Select(wp => wp.piece)
                                  .ToArray();
        handManager.allowedPieces = piecePool;

        handManager.InitHand(stageData);

        // 7) カウンター初期化
        counterPanel.InitCounters(
            stageData.backgroundSprites.Length,
            stageData.RestCount,
            stageData.unlockThresholds
        );
        // 初期解放分も反映
        counterPanel.UpdateCounters(
            _cellsUnlockedTotal,
            _currentBgIndex,
            stageData.unlockThresholds[_currentBgIndex],
            stageData.RestCount - _moves
        );

        // 8) TapOverlay は隠しておく
        tapOverlay.alpha = 0f;
        tapOverlay.gameObject.SetActive(false);

        // ステージ開始時の演出としてすぐスロットを回す
        var slotCtl = Object.FindFirstObjectByType<SlotEffectController>();
        if (slotCtl != null)
            slotCtl.StartSlotEffect();
        else
            Debug.LogWarning("SlotEffectController が見つかりません");

    }

    public void OnPiecePlaced(int unlockedThisMove)
    {
        if (_state != State.Playing) return;

        _moves++;
        _cellsUnlockedTotal += unlockedThisMove;
        counterPanel.UpdateCounters(_cellsUnlockedTotal, _currentBgIndex, stageData.unlockThresholds[_currentBgIndex], stageData.RestCount - _moves);

        bool hasNextBg = _currentBgIndex + 1 < stageData.backgroundSprites.Length;
        int threshold = stageData.unlockThresholds[_currentBgIndex];

        if (_cellsUnlockedTotal >= threshold)
        {
            if (hasNextBg)
            {
                _state = State.WaitingForTap;
                StartCoroutine(HideGridWithEffect());
            }
            else
            {
                // 最終背景でクリア: マスクとミノを隠す
                _state = State.StageClear;
                gridManager.HideAllBlocks();
                gridManager.HideAllMasks();
                counterPanel.ShowStageClear();
            }
            return;
        }

        if (stageData.RestCount > 0 && _moves >= stageData.RestCount)
        {
            GameOver();
            return;
        }

        if (!handManager.HasPlacablePiece(gridManager))
        {
            GameOver();
        }
    }

    IEnumerator HideGridWithEffect()
    {
        // 1) EffectsCanvas を探す
        var effectsCanvasGO = GameObject.Find("EffectsCanvas");
        if (effectsCanvasGO == null)
        {
            Debug.LogError("[GameManager] EffectsCanvas が見つかりません！");
        }
        else
        {
            var effectsRoot = effectsCanvasGO.transform;

            if (explosionPrefab != null)
            {
                // 2) Prefab を UI Canvas (= Screen Space–Camera) の子として生成
                var psGO = Instantiate(
                    explosionPrefab.gameObject,      // Prefab の GameObject
                    Vector3.zero,                    // 位置はローカルでゼロに
                    Quaternion.identity,
                    effectsRoot                      // ← ここで親を指定！
                );

                // 3) RectTransform を中央にリセット（Screen Space–Camera 上での中央）
                var rt = psGO.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = Vector2.zero;
                    rt.localScale = Vector3.one;
                }

                // 4) Canvas 内で最上位に移動
                psGO.transform.SetAsLastSibling();

                // 5) ParticleSystem を再生＆Duration 待機
                var ps = psGO.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    var main = ps.main;
                    float wait = main.duration + main.startLifetime.constantMax;
                    yield return new WaitForSeconds(wait);
                }
                else
                {
                    Debug.LogWarning("[GameManager] explosionPrefab に ParticleSystem がありません");
                    yield return new WaitForSeconds(0.3f);
                }
            }
            else
            {
                Debug.LogWarning("[GameManager] explosionPrefab が Inspector にセットされていません");
                yield return new WaitForSeconds(0.3f);
            }
        }

        // 6) ゲーム盤面のピースとマスクを隠す
        gridManager.HideAllBlocks();
        gridManager.HideAllMasks();

        // 7) TapOverlay を有効化してフェードイン
        tapOverlay.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(tapOverlay, 0f, 1f, 0.3f));
    }

    /// <summary>タップ後に呼ばれる</summary>
    public void ContinueAfterTap()
    {
        if (_state != State.WaitingForTap) return;
        _state = State.Playing;

        // オーバーレイをフェードアウト
        StartCoroutine(FadeCanvasGroup(
            tapOverlay, 1f, 0f, 0.3f,
            () => tapOverlay.gameObject.SetActive(false)
        ));

        // 背景インデックスを進める
        _currentBgIndex++;

        // 新背景設定＋マスク再ロック
        UpdateBackgroundSprite();
        gridManager.LockAllMasks();

        //この処理をスタートストップに割り振る
        /*
        // 追加でランダム解除
        int unlocked = gridManager.UnlockRandomCells(
            Random.Range(1, 20)
        );
        _cellsUnlockedTotal += unlocked;
        */

        // カウンター更新
        counterPanel.UpdateCounters(
            _cellsUnlockedTotal,
            _currentBgIndex,
            stageData.unlockThresholds[_currentBgIndex],
            stageData.RestCount - _moves
        );


        // ミノ再表示
        gridManager.ShowAllBlocks();

        var slotCtl = Object.FindFirstObjectByType<SlotEffectController>();
        if (slotCtl != null)
            slotCtl.ShowPopup();

        
    }

    void UpdateBackgroundSprite()
    {
        if (stageData.backgroundSprites?.Length > 0)
        {

            // ① まず BackgroundMasterData から当該ステージ分を絞り込み
            Sprite newSprite = null;
            if (backgroundMasterData != null)
            {
                var list = backgroundMasterData.records
                            .Where(r => r.stageID == stageData.stageID)
                            .OrderBy(r => r.backgroundID)
                            .ToList();

                if (_currentBgIndex < list.Count)
                    newSprite = list[_currentBgIndex].backgroundImage;
                else
                    Debug.LogWarning($"[GameManager] stageID={stageData.stageID} の背景マスターに index={_currentBgIndex} がありません");
            }

            // ② フォールバック：StageData 側の配列を使いたい場合
            if (newSprite == null && stageData.backgroundSprites != null && stageData.backgroundSprites.Length > 0)
            {
                int idx = Mathf.Clamp(_currentBgIndex, 0, stageData.backgroundSprites.Length - 1);
                newSprite = stageData.backgroundSprites[idx];
            }

            // ③ 画像差し替え
            var bgImg = gridManager.transform.Find("BG_Image").GetComponent<UnityEngine.UI.Image>();
            if (bgImg != null && newSprite != null)
                bgImg.sprite = newSprite;

            var sprite = stageData.backgroundSprites[_currentBgIndex];

            bgImg = gridManager.transform.Find("BG_Image").GetComponent<Image>();

            if (bgImg != null) bgImg.sprite = sprite;
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration, System.Action onComplete = null)
    {
        float t = 0f;
        cg.alpha = from;
        cg.gameObject.SetActive(true);
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
        onComplete?.Invoke();
    }

    void GameOver()
    {
        _state = State.GameOver;
        counterPanel.ShowGameOver();
    }

    public void AddUnlockedCellCount(int count)
    {
        _cellsUnlockedTotal += count;

        // カウンターUI更新
        counterPanel.UpdateCounters(
            _cellsUnlockedTotal,
            _currentBgIndex,
            stageData.unlockThresholds[_currentBgIndex],
            stageData.RestCount - _moves
        );
    }
}
