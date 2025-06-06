// Assets/Scripts/Data/StageMasterRecord.cs
using UnityEngine;

[System.Serializable]
public class StageMasterRecord
{
    [Tooltip("ステージを一意に識別する ID")]
    public int stageID;

    [Tooltip("限定ステージなら true")]
    public bool isLimited;

    [Tooltip("マップの階層 (1,2,3… のように)")]
    public int tier;

    [Tooltip("このステージに割り当てられている背景枚数")]
    public int backgroundCount;

    [Tooltip("ステージセレクト時に表示するサムネイル")]
    public Sprite thumbnail;       // ← ここを追加

    [Tooltip("このステージに対応するアイドルID")]
    public int idolID;    // ← 追加

    [Tooltip("ゲーム本編で使うデータ")]
    public StageData stageData;
}