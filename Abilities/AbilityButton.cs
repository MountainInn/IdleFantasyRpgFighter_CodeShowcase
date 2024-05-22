using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UniRx;
using UniRx.Triggers;

public class AbilityButton : Button,
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] TextMeshProUGUI nameLabel;
    [Space]
    [SerializeField] public UnityEvent onPointerDown, onPointerUp, onPointerClick;

    public void SetInteractable(bool toggle)
    {
        interactable = toggle;
    }

    internal void Connect(Ability ability)
    {
        nameLabel = GetComponentInChildren<TextMeshProUGUI>();
        nameLabel.text = ability.name;

        ability.ObserveReadyToUse()
            .Subscribe(b => SetInteractable(b))
            .AddTo(this);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable)
            return;

        base.OnPointerClick(eventData);

        onPointerClick?.Invoke();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
            return;

        base.OnPointerDown(eventData);

        onPointerDown?.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        onPointerUp?.Invoke();
    }
}
