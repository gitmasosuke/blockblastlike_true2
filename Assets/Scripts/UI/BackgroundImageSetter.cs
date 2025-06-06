using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageSetter : MonoBehaviour
{
    [Header("�w�i�Ɏg�������X�v���C�g���Z�b�g")]
    public Sprite backgroundSprite;

    Image _img;

    void Awake()
    {
        if (backgroundSprite == null)
        {
            Debug.LogWarning("BackgroundImageSetter: backgroundSprite �����ݒ�ł��B");
            return;
        }

        // �� �q�I�u�W�F�N�g���쐬
        var go = new GameObject("BG_Image", typeof(RectTransform));
        go.transform.SetParent(transform, false);

        // �� Image �� CanvasRenderer ���q�ɕt�^
        var cr = go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();

        // �� �X�v���C�g�Ɛݒ�
        img.sprite = backgroundSprite;
        img.type = Image.Type.Simple;
        img.preserveAspect = false;
        img.raycastTarget = false;

        // �� RectTransform ��e�T�C�Y�Ƀt�B�b�g
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // �� �`�揇�F�q�I�u�W�F�N�g�Ȃ̂Ő�ɕ`����AGridLinesDrawer ���w�ʂ�
        go.transform.SetSiblingIndex(0);

        var cvs = go.AddComponent<Canvas>();
        cvs.overrideSorting = true;
        cvs.sortingOrder = -100;
    }

    /// <summary>�w�i�X�v���C�g�������ւ���</summary>
    public void SetSprite(Sprite s)
    {
        if (_img != null) _img.sprite = s;
    }
}
