// Assets/Scripts/Debug/GridDebug.cs
using UnityEngine;

public class GridDebug : MonoBehaviour
{
    public GridManager grid;          // Inspector で GridManager をセット

    void Update()
    {
    }

    void Dump()
    {
        // 盤面上に残っている Block 座標を列挙
        Debug.Log("=== Blocks still alive ===");
        foreach (var blk in grid.GetComponentsInChildren<Block>())
            Debug.Log($"Block at ({blk.x},{blk.y})");

        // fullRow / fullCol をログ出力するためにリフレクションで取り出す
        var gmType = typeof(GridManager);
        var rowField = gmType.GetField("fullRow",
                         System.Reflection.BindingFlags.NonPublic |
                         System.Reflection.BindingFlags.Instance);
        var colField = gmType.GetField("fullCol",
                         System.Reflection.BindingFlags.NonPublic |
                         System.Reflection.BindingFlags.Instance);

        if (rowField != null && colField != null)
        {
            bool[] rows = (bool[])rowField.GetValue(grid);
            bool[] cols = (bool[])colField.GetValue(grid);

            Debug.Log("=== fullRow ===");
            for (int y = 0; y < rows.Length; y++)
                if (rows[y]) Debug.Log($"Row {y} is FULL");

            Debug.Log("=== fullCol ===");
            for (int x = 0; x < cols.Length; x++)
                if (cols[x]) Debug.Log($"Col {x} is FULL");
        }
        else
        {
            Debug.LogWarning("fullRow/fullCol not found—did you rename variables?");
        }
    }
}
