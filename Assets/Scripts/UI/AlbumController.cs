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
    public static int SelectedIdollID { get; private set; }

    void Start()
    {

        // --- nullチェック ---
        //if (bgMaster == null) Debug.LogError("bgMaster が Inspector 未設定です");
        if (idollMaster == null) Debug.LogError("idollMaster が Inspector 未設定です");
        if (contentParent == null) Debug.LogError("contentParent が Inspector 未設定です");
        if (albumItemPrefab == null) Debug.LogError("thumbPrefab が Inspector 未設定です");
        //if (bgMaster == null || idollMaster == null || contentParent == null || thumbPrefab == null)
        //    return; // いずれか不足していたら以降実行しない

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


        foreach (var entry in albumMaster.Entries)
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

            Debug.Log("idollID" + entry.idollID);

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
            var btn = go.transform.Find("Button")?.GetComponent<Button>();

            //int id = entry.idollID;
            btn.onClick.AddListener(() => {
                SelectedIdollID = entry.idollID;

                // ← ここで PlayerPrefs にも保存
                PlayerPrefs.SetInt("AlbumSelectedIdoll", entry.idollID);
                PlayerPrefs.Save();

                Debug.Log("AlbumDetailSceneロード！アイドルID="+ SelectedIdollID);

                SceneManager.LoadScene("AlbumDetailScene");
            });
        }
    }


}