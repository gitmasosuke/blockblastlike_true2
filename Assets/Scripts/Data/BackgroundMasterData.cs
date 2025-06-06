// Assets/Scripts/Data/BackgroundMasterData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "BackgroundMasterData",
    menuName = "Game/Background Master Data",
    order = 12
)]
public class BackgroundMasterData : ScriptableObject
{
    [Tooltip("‚·‚×‚Ä‚Ì”wŒiƒŒƒR[ƒh")]
    public List<BackgroundMasterRecord> records = new List<BackgroundMasterRecord>();
}