using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public abstract class FloatingTextBase : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI label;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] public UnityEvent onEndTween;

    protected float duration;
    protected Vector3 velocity;
    protected RectTransform rect => transform as RectTransform;
    protected Vector3 endPosition => rect.anchoredPosition3D + velocity * duration;

    protected Color startingColor;

    protected void OnValidate()
    {
        Debug.DrawLine(rect.anchoredPosition3D, endPosition, Color.green, 1);
    }

    protected void Awake()
    {
        startingColor = label.color;
    }

    protected void ResetColor()
    {
        SetColor(startingColor);
    }

    public void SetColor(Color color)
    {
        label.color = color;
    }

    public void StartTween()
    {
        DOTween.Sequence()
            .Join(rect
                  .DOAnchorPos(endPosition, duration)
                  .SetEase(Ease.OutBack, 2))
            .Join(label
                  .DOColor(new Color(.3f,.3f,.3f), duration)
                  .SetEase(Ease.InOutQuint))
            .Join(canvasGroup
                  .DOFade(0, duration)
                  .SetEase(Ease.InOutQuint))
            .OnKill(onEndTween.Invoke);
    }
}
