// Assets/Editor/StageDataImporter.cs
using UnityEngine;
using UnityEditor;
using System.Linq;

public class StageDataImporter : EditorWindow
{
    TextAsset csvAsset;
    StageData target;

    [MenuItem("Tools/StageData CSV インポート")]
    static void Open() => GetWindow<StageDataImporter>();

    void OnGUI()
    {
        target = (StageData)EditorGUILayout.ObjectField("StageData", target, typeof(StageData), false);
        csvAsset = (TextAsset)EditorGUILayout.ObjectField("CSV ファイル", csvAsset, typeof(TextAsset), false);

        if (GUILayout.Button("インポート実行") && target != null && csvAsset != null)
        {
            ImportFromCsv(target, csvAsset);
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }

    void ImportFromCsv(StageData sd, TextAsset csv)
    {
        // CSVフォーマット例：
        // initial_blocks: 1,0;2,0;5,10
        // allowed_pieces: I,J,L,S,Z
        var lines = csv.text.Split('\n');
        foreach (var line in lines)
        {
            var kv = line.Split(':');
            if (kv.Length != 2) continue;
            var key = kv[0].Trim();
            var val = kv[1].Trim();

            if (key == "initial_blocks")
            {
                var parts = val.Split(';');
                sd.initialBlockCells = System.Array.ConvertAll(parts, s =>
                {
                    var xy = s.Split(',');
                    return new Vector2Int(
                        int.Parse(xy[0].Trim()),
                        int.Parse(xy[1].Trim())
                    );
                });
            }
            else if (key == "allowed_pieces")
            {
                var names = val.Split(',');

                sd.allowedPieces = names
                    .Select(s => {
                        var tok = s.Trim().Split(':');
                        var key = tok[0].Trim();
                        var pd = Resources.Load<PieceData>(key);
                        if (pd == null)
                            Debug.LogWarning($"PieceData '{key}' が見つかりません");

                        int weight = 1;
                        if (tok.Length > 1 && int.TryParse(tok[1], out var w))
                            weight = Mathf.Max(1, w);

                        return new WeightedPiece
                        {
                            piece = pd,
                            weight = weight
                        };
                    })
                    .ToArray();
            }
        }
    }
}
