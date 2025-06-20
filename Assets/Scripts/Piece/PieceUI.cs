using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PieceUI : MonoBehaviour
{
    [HideInInspector] public PieceData data;

    public const float ThumbCellSize = 50f;

    public float CellSizeInHand => ThumbCellSize;

    public void Init(PieceData d)
    {
        data = d;

        // ensure an Image component exists for raycast purposes
        var imgRoot = GetComponent<Image>();
        if (imgRoot == null)
            imgRoot = gameObject.AddComponent<Image>();
        imgRoot.sprite = null;
        imgRoot.color = new Color(1, 1, 1, 0);
        imgRoot.raycastTarget = true;

        // calculate piece bounds to size the rect correctly
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        foreach (var c in data.cells)
        {
            if (c.x > maxX) maxX = c.x;
            if (c.y > maxY) maxY = c.y;
        }
        var rtRoot = GetComponent<RectTransform>();
        rtRoot.pivot = Vector2.zero; // pivot at lower-left so child cells align
        rtRoot.sizeDelta = new Vector2(
            (maxX + 1) * ThumbCellSize,
            (maxY + 1) * ThumbCellSize);

        // spawn child blocks
        foreach (var c in data.cells)
        {
            var go = new GameObject("ThumbBlock", typeof(RectTransform));
            go.transform.SetParent(transform, false);

            var img = go.AddComponent<Image>();
            img.sprite = d.blockSprite;
            img.preserveAspect = true;
            img.raycastTarget = false;

            var rt = go.GetComponent<RectTransform>();
            rt.pivot = Vector2.zero;
            rt.sizeDelta = Vector2.one * ThumbCellSize;
            rt.anchoredPosition = new Vector2(
                c.x * ThumbCellSize,
                c.y * ThumbCellSize
            );
        }
    }
}
