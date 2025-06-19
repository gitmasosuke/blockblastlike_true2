using System.Linq;
using UnityEngine;

/// <summary>
/// 手札 3 枚の生成・差し替えを行うマネージャ
/// ─────────────────────────────────────────────
/// ・Scene 起動時 / ステージ開始時に InitHand() を呼ぶ
/// ・ミノ（PieceUI）を盤面に置いたら ReplacePiece() を呼んで
///   手札を常に 3 枚に保つ
/// </summary>
public class HandManager : MonoBehaviour
{
    [Header("手札を並べる親")]
    public RectTransform handRoot;

    [Header("手札ピースのプレハブ (PieceUI)")]
    public PieceUI pieceUIPrefab;

    const int HAND_SIZE = 3;

    // ステージ設定を受け取って保持する
    StageData _stageData;

    [Header("このステージで出現可能な PieceData")]
    public PieceData[] allowedPieces;

    /// <summary>ステージ開始時に呼ぶ</summary>
    public void InitHand(StageData stageData)
    {
        Debug.Log("InitHand");

        _stageData = stageData;

        // 既存の手札をクリア
        foreach (Transform t in handRoot)
            Destroy(t.gameObject);

        // 必要枚数だけ生成
        for (int i = 0; i < HAND_SIZE; i++)
            CreatePiece();
    }

    /// <summary>
    /// 盤面に配置し終わった PieceUI を渡すと、
    /// 破棄して新しいピースを補充する
    /// </summary>
    public void ReplacePiece(PieceUI used)
    {
        if (used != null)
            Destroy(used.gameObject);

        CreatePiece();
    }

    /// <summary>手札ピースを 1 つ生成して handRoot に並べる</summary>
    void CreatePiece()
    {
        if (_stageData == null ||
            _stageData.allowedPieces == null ||
            _stageData.allowedPieces.Length == 0)
        {
            Debug.LogError("ステージ設定の allowedPieces が空です");
            return;
        }

        // 重み付きランダムで PieceData を選択
        var data = allowedPieces[Random.Range(0, allowedPieces.Length)];
        if (data == null) return;

        var ui = Instantiate(pieceUIPrefab, handRoot);
        ui.Init(data);
    }

    /// <summary>
    /// ステージデータの allowedPieces から、
    /// weight に従った重み付きランダムで PieceData を返す
    /// </summary>
    PieceData PickWeightedRandomPiece()
    {
        // 1) 合計ウェイトを計算
        int total = 0;
        foreach (var wp in _stageData.allowedPieces)
            total += Mathf.Max(wp.weight, 0);

        // 2) 乱数を引く [0, total)
        int r = Random.Range(0, total);

            for (int x = GridManager.OffsetX; x < GridManager.OffsetX + GridManager.Width; x++)
                for (int y = GridManager.OffsetY; y < GridManager.OffsetY + GridManager.Height; y++)
        foreach (var wp in _stageData.allowedPieces)
        {
            sum += Mathf.Max(wp.weight, 0);
            if (r < sum)
                return wp.piece;
        }

        // 万一のフォールバック
        return _stageData.allowedPieces.Last().piece;
    }

    /// <summary>
    /// 手札のいずれかが盤面のどこかに置けるかチェック
    /// </summary>
    public bool HasPlacablePiece(GridManager grid)
    {
        foreach (Transform t in handRoot)
        {
            var ui = t.GetComponent<PieceUI>();
            if (ui == null) continue;

            // 盤面全セルを走査して一カ所でも置ければ true
            for (int x = 0; x < GridManager.Width; x++)
                for (int y = 0; y < GridManager.Height; y++)
                    if (grid.CanPlace(ui.data.cells, new Vector2Int(x, y)))
                        return true;
        }
        return false;   // すべて置けない
    }

}