using UnityEngine;

/// <summary>
/// ScriptableObject that defines a piece's shape and block sprite
/// </summary>
[CreateAssetMenu(menuName = "BlockBrust/Piece Data")]
public class PieceData : ScriptableObject
{
    [Tooltip("Local cell positions relative to pivot (int grid coords)")]
    public Vector2Int[] cells;

    public Sprite blockSprite;

    public Sprite thumbnailSprite;  // š’Ç‰Ái”CˆÓ‚Åİ’èj
}