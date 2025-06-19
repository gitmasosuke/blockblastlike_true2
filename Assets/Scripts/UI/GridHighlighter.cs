using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridHighlighter : MonoBehaviour
{
    [SerializeField] private Sprite highlightSprite;  // 1×1 白 PNG
    [SerializeField] private Color okColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private Color ngColor = new Color(1f, 0f, 0f, 0.25f);

    // 以前は生成したオブジェクトを都度破棄していたが、
    // モバイル端末では GC 負荷が大きくなるためプール方式に変更
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
        Hide();

        var grid = GameManager.Instance.gridManager;
        var rtGrid = grid.GetComponent<RectTransform>();
        float cs = grid.CellSize;
        float halfW = rtGrid.rect.width * 0.5f;
        float halfH = rtGrid.rect.height * 0.5f;

        EnsurePoolSize(cells.Length);

        Color col = ok ? okColor : ngColor;
        int index = 0;
        // 盤面とのズレを補正するため、表示位置をオフセット
        const int offsetX = 0;
        const int offsetY = 0;
        foreach (var c in cells)
        {
            int x = origin.x + c.x + offsetX;
            int y = origin.y + c.y + offsetY;
            if (x < 0 || x >= GridManager.Width || y < 0 || y >= GridManager.Height)
                continue;

            var img = _pool[index++];
            var rt = img.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = Vector2.one * cs;
            rt.anchoredPosition = new Vector2(
                x * cs - halfW + (cs * 0.5f),
                y * cs - halfH + (cs * 0.5f)
            );

            img.color = col;
            img.enabled = true;
        }
    }

    /// <summary>全ハイライトを非表示にします。</summary>
    public void Hide()
    {
        foreach (var img in _pool) img.enabled = false;
    }

    void EnsurePoolSize(int n)
    {
        while (_pool.Count < n)
        {
            var go = new GameObject("HL", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(transform, false);
            var img = go.GetComponent<Image>();
            img.sprite = highlightSprite;
            img.raycastTarget = false;
            img.rectTransform.anchorMin = img.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            img.enabled = false;
            _pool.Add(img);
        }
    }

    public void Clear()
    {
        Hide();
    }
}
