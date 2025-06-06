using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;               // �� �ǉ�

public class CounterPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text pictureText;
    [SerializeField] private TMP_Text restText;
    [SerializeField] private TMP_Text clearText;

    // �V�K�F�X�e�[�^�X�o�[�p
    [Header("�X�e�[�^�X�o�[�\��")]
    public Slider clearSlider;    // �Z�����臒l�܂ł̐i���p
    public Slider restSlider;         // �萔�c����\���o�[ (0�`�ő�萔)

    private int _maxBackgrounds;
    private int _currentThreshold;

    private int _totalBackgrounds;
    private int _restCount;
    private int[] _unlockThresholds;

    /// <summary>
    /// �X�e�[�W�J�n���ɌĂ�
    /// </summary>
    /// <param name="totalBackgrounds">StageData.backgroundSprites.Length</param>
    /// <param name="restCount">StageData.RestCount</param>
    /// <param name="unlockThresholds">StageData.unlockThresholds</param>
    public void InitCounters(int totalBackgrounds, int restCount, int[] unlockThresholds)
    {
        _totalBackgrounds = totalBackgrounds;

        _restCount = restCount;

        _unlockThresholds = unlockThresholds.ToArray(); //�N���A���C���z��R�s�[

        // �ŏ��͉����������Ă��Ȃ��w�i0����
        UpdateCounters(0, 0, _unlockThresholds[0], _restCount);

        if (clearSlider)
        {
            clearSlider.maxValue = unlockThresholds[0]; 
            Debug.Log("unlockThresholds " + unlockThresholds.Length);
            restSlider.value = 0;
        }

        if (restSlider)
        {
            restSlider.maxValue = restCount;
            Debug.Log("restCount " + restCount);
            restSlider.value = 0;
        }
    }

    /// <summary>
    /// �w�i�̐؂�ւ���Z������̂��тɌĂ�
    /// </summary>
    /// <param name="unlockedCells">���܂܂ŉ�������Z������</param>
    /// <param name="currentBgIndex">���ݕ\�����̔w�i�C���f�b�N�X(0�x�[�X)</param>
    public void UpdateCounters(int unlockedCells, int currentBgIndex,int unlockThreshods,int restCount)
    {

        Debug.Log("�e�L�X�g�\������BG�w�i����" + currentBgIndex);

        // 1) pictureText: �u���ݕ\�����̔w�i����(1�x�[�X)�^�S�w�i���v
        int shown = Mathf.Clamp(currentBgIndex + 1, 1, _totalBackgrounds);
        pictureText.text = $" {shown}/{_totalBackgrounds}";

        // 2) restText: �c��萔�͏�ɃX�e�[�W�����l��\��
        restText.text = $"{restCount}";

        // 3) clearText: ����������Z�����^���̔w�i���؂�ւ��臒l

        clearText.text = $"{unlockedCells}/{unlockThreshods}";

        if (clearSlider)
            clearSlider.value = unlockedCells;

        if (restSlider)
            restSlider.value = restCount;
    }

    public void ShowStageClear()
    {
        clearText.text = "STAGE CLEAR!";
    }

    public void ShowGameOver()
    {
        clearText.text = "GAME OVER";
    }


}