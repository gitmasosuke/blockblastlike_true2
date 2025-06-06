// Assets/Scripts/Data/StageMasterData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "StageMasterData",
    menuName = "Game/Stage Master Data",
    order = 10
)]

[System.Serializable]
public class StageRecord
{
    public int stageID;     // ステージ番号
    public bool isLimited;   // 限定フラグ
    public int tier;        // 階層／ティア
    public Sprite thumbnail;   // サムネイル画像
    // …必要であれば背景枚数なども追加…
}

public class StageMasterData : ScriptableObject
{
    [Tooltip("ステージ選択画面で表示するすべてのステージ情報")]
    public List<StageMasterRecord> stages = new List<StageMasterRecord>();
}