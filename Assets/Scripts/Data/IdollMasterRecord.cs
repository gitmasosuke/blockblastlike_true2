// Assets/Scripts/Data/IdollMasterRecord.cs
using UnityEngine;

[System.Serializable]
public class IdollMasterRecord
{
    [Tooltip("アイドルを一意に識別するID")]
    public int idolID;
    [Tooltip("アイドルの表示名")]
    public string idolName;
}