using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime representation of a piece on the board
/// (Instantiated from PieceUI on drop)
/// </summary>
public class Piece : MonoBehaviour
{
    public PieceData data;

    public void Build(PieceData d, Vector2 cellSize)
    {
        data = d;
        foreach (var c in data.cells)
        {
            var go = new GameObject("Block");
            go.transform.SetParent(transform, false);
            var img = go.AddComponent<Image>();
            img.sprite = data.blockSprite;

            var rt = img.rectTransform;
            rt.sizeDelta = cellSize;
            rt.anchoredPosition = new Vector2(c.x * cellSize.x, c.y * cellSize.y);

            // ★ グリッド座標を保存
            var blk = go.AddComponent<Block>();
            blk.x = c.x;
            blk.y = c.y;
        }
    }

    void LateUpdate()
    {
        if (transform.childCount == 0) Destroy(gameObject);
    }
}