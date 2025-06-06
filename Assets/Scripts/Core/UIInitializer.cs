// Assets/Scripts/Core/UIInitializer.cs
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class UIInitializer : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("�������������s�I");


        // �� Canvas �� Scale �� 1,1,1 �ɖ߂�
        var canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas != null)
        {
            var rtCanvas = canvas.GetComponent<RectTransform>();
            rtCanvas.localScale = Vector3.one;
        }

        // �� GridManager �� RectTransform �����Z�b�g
        var gridGO = GameObject.Find("Canvas/GridManager");
        if (gridGO != null)
        {
            var rtGrid = gridGO.GetComponent<RectTransform>();
            rtGrid.pivot = Vector2.zero;
            rtGrid.localRotation = Quaternion.identity;
            rtGrid.localScale = Vector3.one;
            rtGrid.sizeDelta = new Vector2(640, 640);
        }
    }
}