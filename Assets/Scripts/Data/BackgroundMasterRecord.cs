// Assets/Scripts/Data/BackgroundMasterRecord.cs
using UnityEngine;

[System.Serializable]
public class BackgroundMasterRecord
{
    [Tooltip("背景を一意に識別するID")]
    public int backgroundID;

    [Tooltip("背景画像 (Art/Backgrounds 以下のスプライト)")]
    public Sprite backgroundImage;

    [Tooltip("この背景に関連付くアイドルID")]
    public int idolID;

    [Tooltip("この背景が属するステージID")]
    public int stageID;

    [Tooltip("ステージ内でこの背景の枚数")]
    public int countInStage;

    [Tooltip("この背景をクリア済みかどうか")]
    public bool isCleared;

    [Header("未クリア時に表示するマスクスプライト")]
    public Sprite maskSprite;        // ← ここを追加！
}