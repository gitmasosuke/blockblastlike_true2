using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToHomeButton : MonoBehaviour
{
    // Inspector から変更したい場合は [SerializeField] を付けてもOK
    public string sceneName = "HomeScene";

    // Button の OnClick() にこのメソッドを登録します
    public void OnClick_BackHome()
    {
        SceneManager.LoadScene(sceneName);
    }
}