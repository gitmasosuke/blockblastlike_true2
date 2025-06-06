// Assets/Scripts/Settings/StageData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "BlockBrust/StageData")]




public class StageData : ScriptableObject
{
    [Header("ステージ識別子 (0 から始まる番号など)")]
    public int stageID;

    [Tooltip("ステージ中に表示する背景スプライト（順番に切り替わります）")]
    public Sprite[] backgroundSprites;

    [Tooltip("それぞれの背景を切り替える前に達成すべき“セル解放数”")]
    public int[] unlockThresholds;


    [Header("セル単位の初期ブロック配置")]
    public Vector2Int[] initialBlockCells;

    [Header("出現ミノ（重み付き）")]
    public WeightedPiece[] allowedPieces;


    [Header("初期配置ブロック用スプライト")]
    public Sprite initialBlockSprite;   // ← 追加

    [Header("ステージ制限手数")]
    [Tooltip("配置可能な手数の上限。超えたらゲームオーバーになります。")]
    public int RestCount;
}


[System.Serializable]
public struct WeightedPiece
{
    public PieceData piece;  // ミノ本体
    public int weight;       // 出現率の重み（0以上の整数）
}