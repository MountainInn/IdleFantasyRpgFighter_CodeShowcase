using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Tooltip<TModel, TView> : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
    where TModel : MonoBehaviour
    where TView : TooltipView<TModel>
{
    protected TModel model;

    protected TView view;
    protected RectTransform viewRect;

    public virtual bool CanShowUp()
    {
        return true;
    }

    protected void Awake()
    {
        view = FindObjectOfType<TView>(true);
        viewRect = view.GetComponent<RectTransform>();
    }

    public void MainInitTooltip(TModel model)
    {
        this.model = model;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanShowUp())
            return;

        model ??= GetComponentInChildren<TModel>();

        OnPointerEnter();
       
        view.Show(model);
    }

    public abstract void OnPointerEnter();

    public void OnPointerMove(PointerEventData eventData)
    {
        UpdateAnchors(eventData.position);
        viewRect.anchoredPosition = eventData.position;
    }

    public abstract void OnPointerMove();


    protected void UpdateAnchors(Vector2 position)
    {
        viewRect.pivot =
            new Vector2(
                (position.x < Screen.width  / 2) ? 0 : 1,
                (position.y < Screen.height / 2) ? 0 : 1
            );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        view.Hide();

        OnPointerExit();
    }

    public abstract void OnPointerExit();


    void OnDisable()
    {
        view.Hide();
    }
}
