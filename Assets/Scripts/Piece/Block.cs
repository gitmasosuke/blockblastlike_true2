// Assets/Scripts/Piece/Block.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ピースを構成する 1 セル。gridX / gridY を覚えておく
/// </summary>
public class Block : MonoBehaviour
{
    public int x;   // 0..Width-1
    public int y;   // 0..Height-1

    // グリッド上のセル座標を保存
    public int gridX { get; private set; }
    public int gridY { get; private set; }

    /// <summary>
    /// 初期化。GridManager.Place から呼び出されます。
    /// </summary>
    /// <summary>
    /// 初期化。最後の引数でセル色を指定できるようにします。
    /// </summary>
    public void Init(int x, int y, float size, Sprite sprite)
    {
        gridX = x;
        gridY = y;

        // RectTransform 設定 (中心アンカー・中心ピボット)
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(size, size);

        // グリッド中央からのオフセット計算
        var gm = GameManager.Instance.gridManager;
        var gridRt = gm.GetComponent<RectTransform>();
        float halfW = gridRt.rect.width * 0.5f;
        float halfH = gridRt.rect.height * 0.5f;
        rt.anchoredPosition = new Vector2(
            x * size - halfW + (size * 0.5f),
            y * size - halfH + (size * 0.5f)
        );

        // Image を取得 or 追加 & スプライトをセット
        var img = GetComponent<Image>();
        if (img == null) img = gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;
    }
}