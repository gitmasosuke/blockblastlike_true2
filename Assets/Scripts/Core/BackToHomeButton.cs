using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToHomeButton : MonoBehaviour
{
    // Inspector ����ύX�������ꍇ�� [SerializeField] ��t���Ă�OK
    public string sceneName = "HomeScene";

    // Button �� OnClick() �ɂ��̃��\�b�h��o�^���܂�
    public void OnClick_BackHome()
    {
        SceneManager.LoadScene(sceneName);
    }
}