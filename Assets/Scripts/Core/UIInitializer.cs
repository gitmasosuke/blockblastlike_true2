// Assets/Scripts/Core/UIInitializer.cs
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class UIInitializer : MonoBehaviour
{
    void Awake()
    {
        // ■ Canvas の Scale を 1,1,1 に戻す
        // Prefer the explicitly named root canvas, fall back to the first one found
        var canvasGO = GameObject.Find("Canvas");
        var canvas = canvasGO != null ? canvasGO.GetComponent<Canvas>() : null;
        canvas ??= Object.FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            var rtCanvas = canvas.GetComponent<RectTransform>();
            rtCanvas.localScale = Vector3.one;
        }

        // ■ GridManager の RectTransform をリセット
        var gridGO = GameObject.Find("Canvas/GridManager");
        if (gridGO != null)
        {
            var rtGrid = gridGO.GetComponent<RectTransform>();
            // Pivot must remain centered for BoardHighlighter calculations
            rtGrid.pivot = new Vector2(0.5f, 0.5f);
            rtGrid.localRotation = Quaternion.identity;
            rtGrid.localScale = Vector3.one;
            rtGrid.sizeDelta = new Vector2(640, 640);
        }
    }
}