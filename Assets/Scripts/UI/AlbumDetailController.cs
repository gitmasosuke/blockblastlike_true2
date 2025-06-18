using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.CompilerServices;

public class AlbumDetailController : MonoBehaviour
{
    [Header("マスターデータ参照")]
    public BackgroundMasterData bgMaster;    // Inspector でセット
    public IdollMasterData idollMaster;

    [Header("UI 参照")]
    public RectTransform contentParent;   // サムネイルを並べる親
    public GameObject thumbPrefab;     // Image（サムネ用）を持つプレハブ

    void Start()
    {

        Debug.Log("ディティールコントローラー入った");

        int selectedIdollId = PlayerPrefs.GetInt("AlbumSelectedIdoll", 0);

        Debug.Log("アルバムマスタ表示アイドルID" + selectedIdollId);

        // 1) 該当アイドルのレコード抽出
        var records = bgMaster.records
                        .Where(r => r.idolID == selectedIdollId);

        // ログで件数をチェック
        Debug.Log($"[Album] idollID={selectedIdollId} のレコード数: {records.Count()}");

        // 2) 並べる
        foreach (var rec in records)
        {
            var go = Instantiate(thumbPrefab, contentParent);
            go.name = $"Thumb_{rec.backgroundID}";

            // Thumbnail という子オブジェクトに Image がある前提
            var img = go.transform.Find("Thumbnail")?.GetComponent<Image>();
            if (img == null)
            {
                Debug.LogError($"Thumbnail Image が見つかりません: {go.name}");
                continue;
            }

            // 表示するスプライトを選択
            img.sprite = rec.isCleared
                        ? rec.backgroundImage
                        : rec.maskSprite;
            img.enabled = true;
        }
    }
}