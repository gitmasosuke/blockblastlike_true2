using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageSetter : MonoBehaviour
{
    [Header("背景に使いたいスプライトをセット")]
    public Sprite backgroundSprite;

    Image _img;

    void Awake()
    {
        if (backgroundSprite == null)
        {
            Debug.LogWarning("BackgroundImageSetter: backgroundSprite が未設定です。");
            return;
        }

        // ■ 子オブジェクトを作成
        var go = new GameObject("BG_Image", typeof(RectTransform));
        go.transform.SetParent(transform, false);

        // ■ Image と CanvasRenderer を子に付与
        var cr = go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();

        // ■ スプライトと設定
        img.sprite = backgroundSprite;
        img.type = Image.Type.Simple;
        img.preserveAspect = false;
        img.raycastTarget = false;

        // ■ RectTransform を親サイズにフィット
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // ■ 描画順：子オブジェクトなので先に描かれ、GridLinesDrawer より背面に
        go.transform.SetSiblingIndex(0);

        var cvs = go.AddComponent<Canvas>();
        cvs.overrideSorting = true;
        cvs.sortingOrder = -100;
    }

    /// <summary>背景スプライトを差し替える</summary>
    public void SetSprite(Sprite s)
    {
        if (_img != null) _img.sprite = s;
    }
}
