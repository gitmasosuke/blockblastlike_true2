// Assets/Debug/GridRectWatcher.cs
using UnityEngine;

public class GridRectWatcher : MonoBehaviour
{
    RectTransform rt;
    void Awake() => rt = GetComponent<RectTransform>();
    void LateUpdate()
    {
        if (rt.pivot != Vector2.zero || rt.localScale != Vector3.one)
            Debug.LogWarning($"Pivot={rt.pivot}  Scale={rt.localScale}", rt);
    }
}