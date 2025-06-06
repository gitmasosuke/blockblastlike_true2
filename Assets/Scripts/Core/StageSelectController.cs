using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

/// <summary>
/// ステージ選択画面を動的に作成し、
/// タップされたステージ ID を保存してゲームシーンへ遷移するコントローラ
/// </summary>
public class StageSelectController : MonoBehaviour
{
    [Header("マスター参照 (インスペクターでセット)")]
    public StageMasterData masterData;

    [Header("アイドルマスター (ScriptableObject)")]
    public IdollMasterData idollMasterData;

    [Header("UI プレハブ／参照")]
    public RectTransform contentParent;   // ScrollView → Content
    public Button stageButtonPrefab;      // 内に "Thumbnail" Image と Text (TMP でも可) を持つ

    [Header("ゲーム本編シーン名 (インスペクターでセット)")]
    public string gameSceneName = "GameScene";



    void Start()
    {
        // Inspector 設定チェック
        if (masterData == null || idollMasterData == null
         || contentParent == null || stageButtonPrefab == null)
        {
            Debug.LogError("StageSelectController の Inspector 設定が不足しています");
            return;
        }

        // 既存の子オブジェクト (旧ボタン) をクリア
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // マスターのステージレコードを
        // tier → stageID の昇順でソートしてボタンを生成
        var sorted = masterData.stages
                              .OrderBy(r => r.tier)
                              .ThenBy(r => r.stageID);

        foreach (var rec in sorted)
        {
            // ① Prefab からボタンを生成
            var btn = Instantiate(stageButtonPrefab, contentParent);
            btn.name = $"StageBtn_{rec.stageID}";

            // ② メインラベル(Text) にアイドル名をセット
            //    (子オブジェクトに Text が一つだけあることを想定)
            var tmpLabel = btn.GetComponentInChildren<TextMeshProUGUI>();



            if (tmpLabel != null)
            {
                // idolMasterData から名前を引く
                var idolRec = idollMasterData.records
                                 .FirstOrDefault(i => i.idolID == rec.idolID);

                Debug.Log("アイドルNO=" + idolRec.idolID + "アイドル名：" + idolRec.idolName);

                tmpLabel.text = (idolRec != null)
                             ? idolRec.idolName
                             : $"IDOL_{rec.idolID}";
            }
            else { Debug.Log("アイドル is null!"); }

                // ③ サムネイル設定
                var thumb = btn.transform.Find("Thumbnail")?.GetComponent<Image>();
            if (thumb != null && rec.thumbnail != null)
            {
                thumb.sprite = rec.thumbnail;
                thumb.enabled = true;
            }

            // ④ 限定ステージなら枠色を変える例
            if (rec.isLimited)
                btn.image.color = Color.yellow;

            // ⑤ ボタン押下時のリスナー登録
            int capturedStageID = rec.stageID;
            btn.onClick.AddListener(() => OnStageButtonClicked(capturedStageID));
        }
    }

    /// <summary>
    /// ステージ選択ボタンをタップしたときに呼ばれる
    /// </summary>
    /// <param name="stageID">タップされたステージの ID</param>
    void OnStageButtonClicked(int stageID)
    {
        Debug.Log($"ステージ選択: {stageID}");

        // 1) PlayerPrefs に選択ステージを保存
        PlayerPrefs.SetInt("SelectedStage", stageID);
        PlayerPrefs.Save();

        // 2) ゲーム本編シーンをロード
        SceneManager.LoadScene(gameSceneName);
    }
}