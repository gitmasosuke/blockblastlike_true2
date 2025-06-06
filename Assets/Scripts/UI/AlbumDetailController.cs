using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AlbumDetailController : MonoBehaviour
{
    [Header("�}�X�^�[�f�[�^�Q��")]
    public BackgroundMasterData bgMaster;    // Inspector �ŃZ�b�g
    public IdollMasterData idollMaster;

    [Header("UI �Q��")]
    public RectTransform contentParent;   // �T���l�C������ׂ�e
    public GameObject thumbPrefab;     // Image�i�T���l�p�j�����v���n�u

    void Start()
    {
        int selectedIdollId = PlayerPrefs.GetInt("AlbumSelectedIdoll", 0);

        // 1) �Y���A�C�h���̃��R�[�h���o
        var records = bgMaster.records
                        .Where(r => r.idolID == selectedIdollId);

        // ���O�Ō������`�F�b�N
        Debug.Log($"[Album] idollID={selectedIdollId} �̃��R�[�h��: {records.Count()}");

        // 2) ���ׂ�
        foreach (var rec in records)
        {
            var go = Instantiate(thumbPrefab, contentParent);
            go.name = $"Thumb_{rec.backgroundID}";

            // Thumbnail �Ƃ����q�I�u�W�F�N�g�� Image ������O��
            var img = go.transform.Find("Thumbnail")?.GetComponent<Image>();
            if (img == null)
            {
                Debug.LogError($"Thumbnail Image ��������܂���: {go.name}");
                continue;
            }

            // �\������X�v���C�g��I��
            img.sprite = rec.isCleared
                        ? rec.backgroundImage
                        : rec.maskSprite;
            img.enabled = true;
        }
    }
}