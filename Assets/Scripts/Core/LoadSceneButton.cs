using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    // �C���X�y�N�^����ݒ�\�ɂ������ꍇ�� [SerializeField] ���g���Ă� OK
    public string sceneName = "AlbumScene";

    public void OnClick_LoadAlbum()
    {
        SceneManager.LoadScene(sceneName);
    }
}