/*
// Assets/Scripts/UI/AlbumController.cs
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlbumController : MonoBehaviour
{
    [Header("�}�X�^�[�Q��")]
    public AlbumMasterData albumMaster;    // �쐬�ς݂� AlbumMasterData �A�Z�b�g
    public IdollMasterData idollMaster;    // �쐬�ς݂� IdollMasterData �A�Z�b�g

    [Header("UI �Q��")]
    public RectTransform contentParent;    // Scroll View �� Content
    public GameObject albumItemPrefab;   // AlbumItem �v���n�u

    void Start()
    {
        // Inspector �`�F�b�N
        if (albumMaster == null || idollMaster == null ||
            contentParent == null || albumItemPrefab == null)
        {
            Debug.LogError("AlbumController �� Inspector �ݒ肪�s�����Ă��܂��B");
            return;
        }

        // �����̎q���N���A
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // idollID �� ���O �̎������쐬
        var nameMap = idollMaster.records
                        .ToDictionary(e => e.idolID, e => e.idolName);

        // �e Album �G���g���� UI �ɐ���
        foreach (var entry in albumMaster.entries)
        {
            Debug.Log("�A���o�����[�v");

            var go = Instantiate(albumItemPrefab, contentParent);
            go.name = $"AlbumItem_{entry.idollID}";

            // �T���l�C���ݒ�
            var img = go.transform.Find("Thumbnail")?.GetComponent<Image>();
            if (img != null && entry.thumbnail != null)
            {
                img.sprite = entry.thumbnail;
            }
            else { Debug.Log("�T���l�������ꂸ"); }

            // ���O�ݒ�
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

        // --- null�`�F�b�N ---
        //if (bgMaster == null) Debug.LogError("bgMaster �� Inspector ���ݒ�ł�");
        if (idollMaster == null) Debug.LogError("idollMaster �� Inspector ���ݒ�ł�");
        if (contentParent == null) Debug.LogError("contentParent �� Inspector ���ݒ�ł�");
        if (albumItemPrefab == null) Debug.LogError("thumbPrefab �� Inspector ���ݒ�ł�");
        //if (bgMaster == null || idollMaster == null || contentParent == null || thumbPrefab == null)
        //    return; // �����ꂩ�s�����Ă�����ȍ~���s���Ȃ�

        // Inspector �`�F�b�N
        if (albumMaster == null || idollMaster == null ||
            contentParent == null || albumItemPrefab == null)
        {
            Debug.LogError("AlbumController �� Inspector �ݒ肪�s�����Ă��܂��B");
            return;
        }

        // �����̎q���N���A
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // idollID �� ���O �̎������쐬
        var nameMap = idollMaster.records
                        .ToDictionary(e => e.idolID, e => e.idolName);


        foreach (var entry in albumMaster.Entries)
        {
            var go = Instantiate(albumItemPrefab, contentParent);

            go.name = $"AlbumItem_{entry.idollID}";

            // �T���l�C���ݒ�
            var img = go.transform.Find("Thumbnail")?.GetComponent<Image>();
            if (img != null && entry.thumbnail != null)
            {
                img.sprite = entry.thumbnail;
            }
            else { Debug.Log("�T���l�������ꂸ"); }

            Debug.Log("idollID" + entry.idollID);

            // ���O�ݒ�
            var txt = go.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (txt != null)
            {
                if (nameMap.TryGetValue(entry.idollID, out var nm))
                    txt.text = nm;
                else
                    txt.text = $"ID:{entry.idollID}";
            }

            // �{�^���N���b�N���̃��X�i�[�o�^
            var btn = go.transform.Find("Button")?.GetComponent<Button>();

            //int id = entry.idollID;
            btn.onClick.AddListener(() => {
                SelectedIdollID = entry.idollID;

                // �� ������ PlayerPrefs �ɂ��ۑ�
                PlayerPrefs.SetInt("AlbumSelectedIdoll", entry.idollID);
                PlayerPrefs.Save();

                Debug.Log("AlbumDetailScene���[�h�I�A�C�h��ID="+ SelectedIdollID);

                SceneManager.LoadScene("AlbumDetailScene");
            });
        }
    }


}