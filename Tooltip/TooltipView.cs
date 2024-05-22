using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class TooltipView<TModel> : MonoBehaviour
{
    protected void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();

        rect.anchorMin = rect.anchorMax = Vector2.zero;

        Hide();
    }

    public void Show(TModel model)
    {
        UpdateTooltipView(model);

        gameObject.SetActive(true);
    }

    public abstract void UpdateTooltipView(TModel model);

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
