using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    // インスペクタから設定可能にしたい場合は [SerializeField] を使っても OK
    public string sceneName = "AlbumScene";

    public void OnClick_LoadAlbum()
    {
        SceneManager.LoadScene(sceneName);
    }
}