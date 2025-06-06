using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class GridLinesDrawer : MaskableGraphic
{
    public Color lineColor = new Color(1, 1, 1, 0.1f);
    public float thickness = 2f;

    private static readonly Vector2Int kLineOffset = new Vector2Int(+6, +6); // Å© 6ÉZÉãâEÅEè„Ç÷

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float w = rectTransform.rect.width;
        float h = rectTransform.rect.height;
        Vector2 pivotOff = new Vector2(w * rectTransform.pivot.x,h * rectTransform.pivot.y);

        float cellW = w / GridManager.Width;
        float cellH = h / GridManager.Height;

        // êÇíºê¸
        for (int i = 0; i <= GridManager.Width; i++)
            AddLine(vh,
            new Vector2((i + kLineOffset.x) * cellW - pivotOff.x,
            -pivotOff.y),
            new Vector2((i + kLineOffset.x) * cellW - pivotOff.x,
            h - pivotOff.y));

        // êÖïΩê¸
        for (int j = 0; j <= GridManager.Height; j++)
            AddLine(vh,
            new Vector2(-pivotOff.x,
            (j + kLineOffset.y) * cellH - pivotOff.y),
            new Vector2(w - pivotOff.x,
            (j + kLineOffset.y) * cellH - pivotOff.y));
    }

    void AddLine(VertexHelper vh, Vector2 p1, Vector2 p2)
    {
        Vector2 dir = (p2 - p1).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * thickness * 0.5f;

        var v0 = p1 - normal;
        var v1 = p1 + normal;
        var v2 = p2 + normal;
        var v3 = p2 - normal;

        int idx = vh.currentVertCount;
        vh.AddVert(v0, lineColor, Vector2.zero);
        vh.AddVert(v1, lineColor, Vector2.zero);
        vh.AddVert(v2, lineColor, Vector2.zero);
        vh.AddVert(v3, lineColor, Vector2.zero);

        vh.AddTriangle(idx, idx + 1, idx + 2);
        vh.AddTriangle(idx, idx + 2, idx + 3);
    }


}