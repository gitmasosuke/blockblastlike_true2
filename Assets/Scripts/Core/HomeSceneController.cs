using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeSceneController : MonoBehaviour
{
    /// <summary>
    /// ボタンの OnClick に登録します。
    /// </summary>
    public void OnGotoStageSelect()
    {
        Debug.Log("クリックされた");

        // シーン名は実際のステージ選択シーン名に合わせてください
        SceneManager.LoadScene("StageSelect");
    }
}