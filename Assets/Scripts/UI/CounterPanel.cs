using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;               // ← 追加

public class CounterPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text pictureText;
    [SerializeField] private TMP_Text restText;
    [SerializeField] private TMP_Text clearText;

    // 新規：ステータスバー用
    [Header("ステータスバー表示")]
    public Slider clearSlider;    // セル解放閾値までの進捗用
    public Slider restSlider;         // 手数残数を表すバー (0～最大手数)

    private int _maxBackgrounds;
    private int _currentThreshold;

    private int _totalBackgrounds;
    private int _restCount;
    private int[] _unlockThresholds;

    /// <summary>
    /// ステージ開始時に呼ぶ
    /// </summary>
    /// <param name="totalBackgrounds">StageData.backgroundSprites.Length</param>
    /// <param name="restCount">StageData.RestCount</param>
    /// <param name="unlockThresholds">StageData.unlockThresholds</param>
    public void InitCounters(int totalBackgrounds, int restCount, int[] unlockThresholds)
    {
        _totalBackgrounds = totalBackgrounds;

        _restCount = restCount;

        _unlockThresholds = unlockThresholds.ToArray(); //クリアライン配列コピー

        // 最初は何も解放されていない背景0枚目
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
    /// 背景の切り替えやセル解放のたびに呼ぶ
    /// </summary>
    /// <param name="unlockedCells">いままで解放したセル総数</param>
    /// <param name="currentBgIndex">現在表示中の背景インデックス(0ベース)</param>
    public void UpdateCounters(int unlockedCells, int currentBgIndex,int unlockThreshods,int restCount)
    {

        Debug.Log("テキスト表示するBG背景枚数" + currentBgIndex);

        // 1) pictureText: 「現在表示中の背景枚数(1ベース)／全背景数」
        int shown = Mathf.Clamp(currentBgIndex + 1, 1, _totalBackgrounds);
        pictureText.text = $" {shown}/{_totalBackgrounds}";

        // 2) restText: 残り手数は常にステージ初期値を表示
        restText.text = $"{restCount}";

        // 3) clearText: 今解放したセル数／その背景が切り替わる閾値

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