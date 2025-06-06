// Assets/Scripts/Hand/PieceUI.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PieceUI : MonoBehaviour
{
    [HideInInspector] public PieceData data;

    // ��D���� �̂P�Z���T�C�Y�i���D�݂Œ����j
    public const float ThumbCellSize = 50f;

    // �i�K�v�ł���΁j�C���X�^���X�p�̓ǂݎ���p�v���p�e�B���p��
    public float CellSizeInHand => ThumbCellSize;

    public void Init(PieceData d)
    {
        data = d;

            // ���� Image �͓��������ă��C�L���X�g����������
        var oldImg = GetComponent<Image>();
            if (oldImg != null)
                {
            oldImg.sprite = null;                 // �\�����Ȃ�
            oldImg.color = new Color(1, 1, 1, 0);    // ���S����
            oldImg.raycastTarget = true;                 // �C�x���g���󂯎��
                }
            else
                {
                    // ����������Γ��� Image ��ǉ�
            oldImg = gameObject.AddComponent<Image>();
            oldImg.color = new Color(1, 1, 1, 0);
            oldImg.raycastTarget = true;
                }

        // ��D�p�̃Z����`��
        foreach (var c in data.cells)
        {
            var go = new GameObject("ThumbBlock", typeof(RectTransform));
            go.transform.SetParent(transform, false);

            var img = go.AddComponent<Image>();
            img.sprite = d.blockSprite;
            img.preserveAspect = true;
            img.raycastTarget = false;

            var rt = go.GetComponent<RectTransform>();
            rt.pivot = Vector2.zero;
            rt.sizeDelta = Vector2.one * ThumbCellSize;

            // �c�Œ�ō�����Ȃ�A�Z�����W�����̂܂܎g���܂�
            rt.anchoredPosition = new Vector2(
                c.x * ThumbCellSize,
                c.y * ThumbCellSize
            );
        }

        // ��D���m���d�Ȃ�Ȃ��悤�S�̂𒆉��񂹂ɂ������ꍇ�́A
        // RectTransform �� anchoredPosition �𒲐����Ă��������B
    }
}