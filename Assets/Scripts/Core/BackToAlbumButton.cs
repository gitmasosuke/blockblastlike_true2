using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToAlbumButton : MonoBehaviour
{
    // �C���X�y�N�^�ŕς������ꍇ�� [SerializeField] ���g���Ă�OK
    public string sceneName = "AlbumScene";

    // Button �� OnClick() �ɂ��̃��\�b�h��o�^���܂�
    public void OnClick_Back()
    {
        SceneManager.LoadScene(sceneName);
    }
}