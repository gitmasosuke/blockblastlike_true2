using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeSceneController : MonoBehaviour
{
    /// <summary>
    /// �{�^���� OnClick �ɓo�^���܂��B
    /// </summary>
    public void OnGotoStageSelect()
    {
        Debug.Log("�N���b�N���ꂽ");

        // �V�[�����͎��ۂ̃X�e�[�W�I���V�[�����ɍ��킹�Ă�������
        SceneManager.LoadScene("StageSelect");
    }
}