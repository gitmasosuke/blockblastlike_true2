/*
// Assets/Scripts/UI/AlbumController.cs
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlbumController : MonoBehaviour
{
    [Header("マスター参照")]
    public AlbumMasterData albumMaster;    // 作成済みの AlbumMasterData アセット
    public IdollMasterData idollMaster;    // 作成済みの IdollMasterData アセット

    [Header("UI 参照")]
    public RectTransform contentParent;    // Scroll View → Content
    public GameObject albumItemPrefab;   // AlbumItem プレハブ

    void Start()
    {
        // Inspector チェック
        if (albumMaster == null || idollMaster == null ||
            contentParent == null || albumItemPrefab == null)
        {
            Debug.LogError("AlbumController の Inspector 設定が不足しています。");
            return;
        }

        // 既存の子をクリア
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // idollID → 名前 の辞書を作成
        var nameMap = idollMaster.records
                        .ToDictionary(e => e.idolID, e => e.idolName);

        // 各 Album エントリを UI に生成
        foreach (var entry in albumMaster.entries)
        {
            Debug.Log("アルバムループ");

            var go = Instantiate(albumItemPrefab, contentParent);
            go.name = $"AlbumItem_{entry.idollID}";

            // サムネイル設定
            var img = go.transform.Find("Thumbnail")?.GetComponent<Image>();
            if (img != null && entry.thumbnail != null)
            {
                img.sprite = entry.thumbnail;
            }
            else { Debug.Log("サムネ処理されず"); }

            // 名前設定
            var txt = go.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (txt != null)
            {
                if (nameMap.TryGetValue(entry.idollID, out var nm))
                    txt.text = nm;
                else
                    txt.text = $"ID:{entry.idollID}";
            }
        }
    }
}*/

// Assets/Scripts/UI/AlbumController.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class AlbumController : MonoBehaviour
{
    public AlbumMasterData albumMaster;
    public IdollMasterData idollMaster;
    public RectTransform contentParent;
    public GameObject albumItemPrefab;

    // ← ここを追加
    public static int SelectedIdollID { get; private set; }

    void Start()
    {
        // Inspector チェック
        if (albumMaster == null || idollMaster == null ||
            contentParent == null || albumItemPrefab == null)
        {
            Debug.LogError("AlbumController の Inspector 設定が不足しています。");
            return;
        }

        // 既存の子をクリア
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // idollID → 名前 の辞書を作成
        var nameMap = idollMaster.records
                        .ToDictionary(e => e.idolID, e => e.idolName);


        foreach (var entry in albumMaster.entries)
        {
            var go = Instantiate(albumItemPrefab, contentParent);

            go.name = $"AlbumItem_{entry.idollID}";

            // サムネイル設定
            var img = go.transform.Find("Thumbnail")?.GetComponent<Image>();
            if (img != null && entry.thumbnail != null)
            {
                img.sprite = entry.thumbnail;
            }
            else { Debug.Log("サムネ処理されず"); }

            // 名前設定
            var txt = go.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (txt != null)
            {
                if (nameMap.TryGetValue(entry.idollID, out var nm))
                    txt.text = nm;
                else
                    txt.text = $"ID:{entry.idollID}";
            }

            // ボタンクリック時のリスナー登録
            var btn = go.GetComponent<Button>();
            int id = entry.idollID;
            btn.onClick.AddListener(() => {
                SelectedIdollID = id;
                SceneManager.LoadScene("AlbumDetailScene");
            });
        }
    }
}