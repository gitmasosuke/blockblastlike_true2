using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToAlbumButton : MonoBehaviour
{
    // インスペクタで変えたい場合は [SerializeField] を使ってもOK
    public string sceneName = "AlbumScene";

    // Button の OnClick() にこのメソッドを登録します
    public void OnClick_Back()
    {
        SceneManager.LoadScene(sceneName);
    }
}