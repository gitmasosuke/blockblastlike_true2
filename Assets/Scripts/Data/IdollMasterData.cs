// Assets/Scripts/Data/IdollMasterData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(
    fileName = "IdollMasterData",
    menuName = "Game/Idoll Master Data",
    order = 11
)]
public class IdollMasterData : ScriptableObject
{
    [Tooltip("ID �� ���O�̑Ή����X�g")]
    public List<IdollMasterRecord> records = new List<IdollMasterRecord>();
}