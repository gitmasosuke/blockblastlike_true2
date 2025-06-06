// Assets/Scripts/Piece/Block.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �s�[�X���\������ 1 �Z���BgridX / gridY ���o���Ă���
/// </summary>
public class Block : MonoBehaviour
{
    public int x;   // 0..Width-1
    public int y;   // 0..Height-1

    // �O���b�h��̃Z�����W��ۑ�
    public int gridX { get; private set; }
    public int gridY { get; private set; }

    /// <summary>
    /// �������BGridManager.Place ����Ăяo����܂��B
    /// </summary>
    /// <summary>
    /// �������B�Ō�̈����ŃZ���F���w��ł���悤�ɂ��܂��B
    /// </summary>
    public void Init(int x, int y, float size, Sprite sprite)
    {
        gridX = x;
        gridY = y;

        // RectTransform �ݒ� (���S�A���J�[�E���S�s�{�b�g)
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(size, size);

        // �O���b�h��������̃I�t�Z�b�g�v�Z
        var gm = GameManager.Instance.gridManager;
        var gridRt = gm.GetComponent<RectTransform>();
        float halfW = gridRt.rect.width * 0.5f;
        float halfH = gridRt.rect.height * 0.5f;
        rt.anchoredPosition = new Vector2(
            x * size - halfW + (size * 0.5f),
            y * size - halfH + (size * 0.5f)
        );

        // Image ���擾 or �ǉ� & �X�v���C�g���Z�b�g
        var img = GetComponent<Image>();
        if (img == null) img = gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;
    }
}