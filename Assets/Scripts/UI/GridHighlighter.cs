using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridHighlighter : MonoBehaviour
{
    [SerializeField] private Sprite highlightSprite;  // 1×1 白 PNG
    [SerializeField] private Color okColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private Color ngColor = new Color(1f, 0f, 0f, 0.25f);

    private readonly List<RectTransform> _marks = new List<RectTransform>();


    /// <summary>ドラッグ中に生成する Image プール</summary>
    private readonly List<Image> _pool = new();

    private RectTransform _rtGrid;
    private float _cellW, _cellH;


    /// <summary>
    /// 指定のセル群をハイライト表示します。
    /// </summary>
    /// <param name="cells">ピースの相対セル座標配列</param>
    /// <param name="origin">グリッド上の原点セル座標</param>
    /// <param name="ok">配置可能なら true（白）、不可なら false（赤）</param>
    public void Show(Vector2Int[] cells, Vector2Int origin, bool ok)
    {
        Clear();

        var grid = GameManager.Instance.gridManager;
        var rtGrid = grid.GetComponent<RectTransform>();
        float cs = grid.CellSize;
        float halfW = rtGrid.rect.width * 0.5f;
        float halfH = rtGrid.rect.height * 0.5f;

        foreach (var c in cells)
        {
            int x = origin.x + c.x;
            int y = origin.y + c.y;
            if (x < 0 || x >= GridManager.Width || y < 0 || y >= GridManager.Height)
                continue;

            var go = new GameObject($"Hl_{x}_{y}",
                                    typeof(RectTransform),
                                    typeof(CanvasRenderer),
                                    typeof(Image));
            go.transform.SetParent(transform, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = Vector2.one * cs;
            rt.anchoredPosition = new Vector2(
                x * cs - halfW + (cs * 0.5f),
                y * cs - halfH + (cs * 0.5f)
            );

            var img = go.GetComponent<Image>();
            img.sprite = highlightSprite;
            img.preserveAspect = true;
            img.raycastTarget = false;
            img.color = ok ? okColor : ngColor;

            _marks.Add(rt);
        }
    }

    /// <summary>全ハイライトを消す</summary>

    public void Hide()
    {
        foreach (var img in _pool) img.enabled = false;
    }

        void EnsurePoolSize(int n)
    {
        while (_pool.Count < n)
        {
            var go = new GameObject("HL", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var img = go.AddComponent<Image>();
            img.sprite = highlightSprite;
            img.raycastTarget = false;
            img.rectTransform.pivot = Vector2.zero;
            img.rectTransform.sizeDelta = new Vector2(_cellW, _cellH);
            img.enabled = false;
            _pool.Add(img);
        }
    }

    public void Clear()
    {
        foreach (var rt in _marks)
            if (rt != null) Destroy(rt.gameObject);
        _marks.Clear();
    }
}
