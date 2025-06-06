using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // �� �ǉ��iInput�����ł悯��Εs�v�j

public class SlotEffectController : MonoBehaviour
{
    [Header("�Q�Ɛݒ�")]
    public GridManager gridManager; // GridManager �������ɃA�T�C��
    public Button slotButton;       // ���o���J�n����{�^�����A�T�C��

    [Header("�X���b�g�ݒ�")]
    [Tooltip("1���[�v������Ƀv���r���[��������}�X��")]
    public int unlockCount = 10;

    [Tooltip("�v���r���[��؂�ւ���Ԋu(�b)")]
    public float cycleInterval = 0.1f;

    [Tooltip("���o�S�̂̍Đ�����(�b)")]
    public float totalDuration = 2f;

    [Header("�|�b�v�A�b�v�ݒ�")]
    public GameObject slotPopupPrefab;   // ��ō���� SlotPopupPanel.prefab
    private GameObject _popupInstance;
    private Button _btnClose;
    private Button _btnWatchAd;

    Coroutine _slotRoutine;
    List<Vector2Int> _lastPreview;

    List<Vector2Int> currentPreview = new();

    public GameObject tapToNextText;  // �gTap To Next�h �e�L�X�g�iHierarchy �� GameObject�j

    private bool firstSlot = true;

    void Awake()
    {
        // Inspector �ɐݒ肪�Ȃ���΃V�[�����玩������
        if (gridManager == null)
        {
            // Unity 2023.1 �ȍ~�Ȃ�
            gridManager = Object.FindFirstObjectByType<GridManager>();
            // ��������ł� null �Ȃ�x��
            if (gridManager == null)
                Debug.LogError("SlotEffectController: GridManager ��������܂���I");
        }

        if (slotButton != null)
            slotButton.onClick.AddListener(StartSlotEffect);
    }

    /// <summary>
    /// �{�^���������ɌĂ΂��B�����̉��o���~���čăX�^�[�g�B
    /// </summary>
    public void StartSlotEffect()
    {
        if (_slotRoutine != null)
            StopCoroutine(_slotRoutine);

        // �gTap To Next�h �e�L�X�g��\��
        tapToNextText.SetActive(true);

        // ���O�̃v���r���[���c���Ă���΃N���A
        if (_lastPreview != null && _lastPreview.Count > 0)
            gridManager.ClearPreviewUnlocks(_lastPreview);

        _slotRoutine = StartCoroutine(SlotRoutine());
    }

    IEnumerator SlotRoutine()
    {
        float elapsed = 0f;
        List<Vector2Int> preview = new List<Vector2Int>();

        while (elapsed < totalDuration)
        {

            // ���� �@ �O��v���r���[��K���N���A
            if (preview.Count > 0)
            {
                gridManager.ClearPreviewUnlocks(preview);
                preview.Clear();
            }

            // ���� �A �V�����v���r���[�擾���\��
            preview = gridManager.PreviewRandomUnlocks(unlockCount);
            currentPreview = new List<Vector2Int>(preview); //������ǉ�

            // ���� ���o�Ԋu�ҋ@
            yield return new WaitForSeconds(cycleInterval);

            // �B cycleInterval�b�ҋ@
            float waited = 0f;
            while (waited < cycleInterval)
            {
                // �����Łu��ʃ^�b�v�����o�����瑁�������v��OK
                if (Input.GetMouseButtonDown(0))
                {
                    // ���o�I�����̍Ō�̃v���r���[�����̂܂܎c���Ă���
                    _lastPreview = new List<Vector2Int>(currentPreview);

                    StopSlot();

                    // �����ŉ��o�͎~�܂�����ԂȂ̂ŁA�|�b�v�A�b�v���o��
                    if(firstSlot)
                        ShowPopup();


                    yield break;
                }
                waited += Time.deltaTime;
                yield return null;
            }

            elapsed += cycleInterval;
        }

        // ���o�I�������O�̂��߃N���A
        if (preview.Count > 0)
        {
            gridManager.ClearPreviewUnlocks(preview);
            preview.Clear();
        }
        
        // �X���b�g�I�����Ƀv���r���[�͂��̂܂܎c���Ă����i�m��҂��j
        _slotRoutine = null;


    }

    public void StopSlot()
    {
        if (_slotRoutine != null)
        {
            StopCoroutine(_slotRoutine);
            _slotRoutine = null;

            // �@ �v���r���[����
            if (_lastPreview.Count > 0)
                gridManager.ClearPreviewUnlocks(_lastPreview);

            // �Ō�̃v���r���[��Ԃ𐳎��ɉ��
            gridManager.CommitUnlocks(currentPreview);

            //�J�E���g�� GameManager �ɒʒm
            GameManager.Instance.AddUnlockedCellCount(currentPreview.Count);

            // �v���r���[���X�g�����Z�b�g
            currentPreview.Clear();

            // �gTap To Next�h �e�L�X�g���B��
            tapToNextText.SetActive(false);

            _lastPreview.Clear();
        }
    }

    /// <summary>���o�I����Ƀ|�b�v�A�b�v��\��</summary>
    public void ShowPopup()
    {

        //����t���O�N���A
        firstSlot = false;

        if (slotPopupPrefab == null)
        {
            Debug.LogError("slotPopupPrefab ���Z�b�g����Ă��܂���I");
            return;
        }

        // �C���X�^���X��
        _popupInstance = Instantiate(
            slotPopupPrefab,
            gridManager.transform.parent  // Canvas �̒����ɒu��
        );

        // �B RectTransform �𒆉��ɔz�u
        var rt = _popupInstance.GetComponent<RectTransform>();
        if (rt != null)
        {
            // �A���J�[�^�s�{�b�g�𒆐S��
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            // ���[�J�����W(0,0)���L�����o�X���S�ɂȂ�
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;

            // �őO�ʂ�
            _popupInstance.transform.SetAsLastSibling();
        }

        // �{�^���擾
        _btnClose = _popupInstance.transform.Find("ButtonClose").GetComponent<Button>();
        _btnWatchAd = _popupInstance.transform.Find("ButtonWatchAd").GetComponent<Button>();

        // ���X�i�[�o�^
        _btnClose.onClick.AddListener(OnPopupClose);
        _btnWatchAd.onClick.AddListener(OnPopupWatchAd);
    }

    /// <summary>�u����v�������F���̂܂܍ŏI�����{�ԏ������ďI��</summary>
    private void OnPopupClose()
    {
        CommitLastPreviewAndCleanup();
        Destroy(_popupInstance);
    }

    /// <summary>�uCM�����v�������F�ēx���[���b�g���o �� �ŏI��� �� �I��</summary>
    private void OnPopupWatchAd()
    {
        // ��x�|�b�v�A�b�v�����
        Destroy(_popupInstance);

        // �ēx���[���b�g���o���X�^�[�g
        StartSlotEffect();

        // ���������ۂɍL���𗬂��Ȃ炱���ōL���Đ�������ł��� StartSlotEffect() ���Ă�ł�������
    }

    /// <summary>
    /// �Ō�Ƀv���r���[����Ă����Z����{�ԃA�����b�N���Č�Еt��
    /// </summary>
    private void CommitLastPreviewAndCleanup()
    {
        if (_lastPreview != null && _lastPreview.Count > 0)
        {
            gridManager.CommitUnlocks(_lastPreview);
            _lastPreview.Clear();
        }
    }
}