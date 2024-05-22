using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(CanvasGroup))]
public class Fade : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] bool visible;
    [SerializeField] bool interactable;
    [SerializeField] bool blocksRaycast;
    [SerializeField] public float duration;
    [Space]
    [SerializeField] public UnityEvent onFadeIn, onFadeOut;

    void Awake()
    {
        if (visible)
            FadeIn();
        else
            FadeOut();
    }

    public void Toggle()
    {
        if (visible)
            FadeOut();
        else
            FadeIn();
    }

    public async UniTask FadeIn()
    {
        canvasGroup.DOKill();

        await
            canvasGroup
            .DOFade(1, duration)
            .SetEase(Ease.OutQuad)
            .OnKill(() =>
            {
                visible = true;

                ToggleInteractable();

                onFadeIn?.Invoke();
            })
            .AsyncWaitForKill();
    }

    public async UniTask FadeOut()
    {
        canvasGroup.DOKill();

        await
            canvasGroup
            .DOFade(0, duration)
            .SetEase(Ease.OutQuad)
            .OnStart(() =>
            {
                visible = false;

                ToggleInteractable();
               
                onFadeOut?.Invoke();
            })
            .AsyncWaitForKill();
    }

    void ToggleInteractable()
    {
        canvasGroup.blocksRaycasts = (visible && blocksRaycast);
        canvasGroup.interactable = (visible && interactable);
    }
}
