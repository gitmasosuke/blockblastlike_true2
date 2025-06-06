using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    [Header("スタートボタン")]
    public Button startButton;
    [Header("ロードするシーン名")]
    public string nextSceneName = "HomeScene";

    void Awake()
    {
        if (startButton == null)
            Debug.LogError("Start Button が Inspector で未設定です！");
        else
            startButton.onClick.AddListener(OnStartPressed);
    }

    private void OnStartPressed()
    {
        Debug.Log("▶ Start ボタンが押されました！");
        SceneManager.LoadScene(nextSceneName);
    }
}