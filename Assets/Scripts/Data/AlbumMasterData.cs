// Assets/Scripts/Data/AlbumMasterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "AlbumMasterData", menuName = "Masters/AlbumMasterData")]
public class AlbumMasterData : ScriptableObject
{
    [Tooltip("�A���o���ɓo�^����e�A�C�e��")]
    public AlbumEntry[] Entries;
}

[System.Serializable]
public class AlbumEntry
{
    [Tooltip("�A�C�h����ID")]
    public int idollID;

    [Tooltip("�N���A��ɕ\������T���l�C���摜")]
    public Sprite thumbnail;
}