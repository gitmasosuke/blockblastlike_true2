// Assets/Scripts/Hand/PieceUI.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PieceUI : MonoBehaviour
{
    [HideInInspector] public PieceData data;

    // 手札内で の１セルサイズ（お好みで調整）
    public const float ThumbCellSize = 50f;

    // （必要であれば）インスタンス用の読み取り専用プロパティも用意
    public float CellSizeInHand => ThumbCellSize;

    public void Init(PieceData d)
    {
        data = d;

            // 既存 Image は透明化してレイキャストだけ生かす
        var oldImg = GetComponent<Image>();
            if (oldImg != null)
                {
            oldImg.sprite = null;                 // 表示しない
            oldImg.color = new Color(1, 1, 1, 0);    // 完全透明
            oldImg.raycastTarget = true;                 // イベントを受け取る
                }
            else
                {
                    // もし無ければ透明 Image を追加
            oldImg = gameObject.AddComponent<Image>();
            oldImg.color = new Color(1, 1, 1, 0);
            oldImg.raycastTarget = true;
                }

        // 手札用のセルを描画
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

            // 縦固定で左下基準なら、セル座標をそのまま使えます
            rt.anchoredPosition = new Vector2(
                c.x * ThumbCellSize,
                c.y * ThumbCellSize
            );
        }

        // 手札同士が重ならないよう全体を中央寄せにしたい場合は、
        // RectTransform の anchoredPosition を調整してください。
    }
}