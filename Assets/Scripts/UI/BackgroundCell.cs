using UnityEngine;
using UnityEngine.UI;

public class BackgroundCell : MonoBehaviour
{
    Image _mask;               // マスク用イメージ
    bool _permanentLocked;    // 永続ロック状態

    void Awake()
    {
        // 自前 Image は無効化
        var selfImg = GetComponent<Image>();
        if (selfImg != null) selfImg.enabled = false;

        // マスク用イメージを子に生成
        var go = new GameObject("Mask", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);
        _mask = go.GetComponent<Image>();
        _mask.color = new Color(0.36f, 0.36f, 0.36f, 0.98f);
        _mask.raycastTarget = false;
        var rt = _mask.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        // 初期状態は「永久ロック中」
        _permanentLocked = true;
        _mask.enabled = true;
    }

    /// <summary>論理的にロック中か（プレビューでは変わらない）</summary>
    public bool IsLocked => _permanentLocked;

    /// <summary>本当にマスクをはずす（Permanent Unlock）</summary>
    public void Unlock()
    {
        _permanentLocked = false;
        _mask.enabled = false;
    }

    /// <summary>本当にマスクをかけ直す（Permanent Lock）</summary>
    public void Lock()
    {
        _permanentLocked = true;
        _mask.enabled = true;
    }

    /// <summary>プレビュー用に一時的にマスクを外す（論理状態は変えない）</summary>
    public void PreviewUnlock()
    {
        _mask.enabled = false;
    }

    /// <summary>プレビューを取り消して、本来のロック状態に戻す</summary>
    public void ClearPreview()
    {
        _mask.enabled = _permanentLocked;
    }
}