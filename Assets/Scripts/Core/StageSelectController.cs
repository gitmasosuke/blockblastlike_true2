using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

/// <summary>
/// �X�e�[�W�I����ʂ𓮓I�ɍ쐬���A
/// �^�b�v���ꂽ�X�e�[�W ID ��ۑ����ăQ�[���V�[���֑J�ڂ���R���g���[��
/// </summary>
public class StageSelectController : MonoBehaviour
{
    [Header("�}�X�^�[�Q�� (�C���X�y�N�^�[�ŃZ�b�g)")]
    public StageMasterData masterData;

    [Header("�A�C�h���}�X�^�[ (ScriptableObject)")]
    public IdollMasterData idollMasterData;

    [Header("UI �v���n�u�^�Q��")]
    public RectTransform contentParent;   // ScrollView �� Content
    public Button stageButtonPrefab;      // ���� "Thumbnail" Image �� Text (TMP �ł���) ������

    [Header("�Q�[���{�҃V�[���� (�C���X�y�N�^�[�ŃZ�b�g)")]
    public string gameSceneName = "GameScene";



    void Start()
    {
        // Inspector �ݒ�`�F�b�N
        if (masterData == null || idollMasterData == null
         || contentParent == null || stageButtonPrefab == null)
        {
            Debug.LogError("StageSelectController �� Inspector �ݒ肪�s�����Ă��܂�");
            return;
        }

        // �����̎q�I�u�W�F�N�g (���{�^��) ���N���A
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // �}�X�^�[�̃X�e�[�W���R�[�h��
        // tier �� stageID �̏����Ń\�[�g���ă{�^���𐶐�
        var sorted = masterData.stages
                              .OrderBy(r => r.tier)
                              .ThenBy(r => r.stageID);

        foreach (var rec in sorted)
        {
            // �@ Prefab ����{�^���𐶐�
            var btn = Instantiate(stageButtonPrefab, contentParent);
            btn.name = $"StageBtn_{rec.stageID}";

            // �A ���C�����x��(Text) �ɃA�C�h�������Z�b�g
            //    (�q�I�u�W�F�N�g�� Text ����������邱�Ƃ�z��)
            var tmpLabel = btn.GetComponentInChildren<TextMeshProUGUI>();



            if (tmpLabel != null)
            {
                // idolMasterData ���疼�O������
                var idolRec = idollMasterData.records
                                 .FirstOrDefault(i => i.idolID == rec.idolID);

                Debug.Log("�A�C�h��NO=" + idolRec.idolID + "�A�C�h�����F" + idolRec.idolName);

                tmpLabel.text = (idolRec != null)
                             ? idolRec.idolName
                             : $"IDOL_{rec.idolID}";
            }
            else { Debug.Log("�A�C�h�� is null!"); }

                // �B �T���l�C���ݒ�
                var thumb = btn.transform.Find("Thumbnail")?.GetComponent<Image>();
            if (thumb != null && rec.thumbnail != null)
            {
                thumb.sprite = rec.thumbnail;
                thumb.enabled = true;
            }

            // �C ����X�e�[�W�Ȃ�g�F��ς����
            if (rec.isLimited)
                btn.image.color = Color.yellow;

            // �D �{�^���������̃��X�i�[�o�^
            int capturedStageID = rec.stageID;
            btn.onClick.AddListener(() => OnStageButtonClicked(capturedStageID));
        }
    }

    /// <summary>
    /// �X�e�[�W�I���{�^�����^�b�v�����Ƃ��ɌĂ΂��
    /// </summary>
    /// <param name="stageID">�^�b�v���ꂽ�X�e�[�W�� ID</param>
    void OnStageButtonClicked(int stageID)
    {
        Debug.Log($"�X�e�[�W�I��: {stageID}");

        // 1) PlayerPrefs �ɑI���X�e�[�W��ۑ�
        PlayerPrefs.SetInt("SelectedStage", stageID);
        PlayerPrefs.Save();

        // 2) �Q�[���{�҃V�[�������[�h
        SceneManager.LoadScene(gameSceneName);
    }
}