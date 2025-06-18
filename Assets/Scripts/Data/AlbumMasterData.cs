// Assets/Scripts/Data/AlbumMasterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "AlbumMasterData", menuName = "Masters/AlbumMasterData")]
public class AlbumMasterData : ScriptableObject
{
    [Tooltip("アルバムに登録する各アイテム")]
    public AlbumEntry[] Entries;
}

[System.Serializable]
public class AlbumEntry
{
    [Tooltip("アイドルのID")]
    public int idollID;

    [Tooltip("クリア後に表示するサムネイル画像")]
    public Sprite thumbnail;
}