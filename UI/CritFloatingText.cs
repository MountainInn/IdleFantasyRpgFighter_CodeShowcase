using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class CritFloatingText : FloatingTextBase
{
    public class Pool : MonoMemoryPool<string, Vector3, float, Vector3, CritFloatingText>
    {
        protected override void OnCreated(CritFloatingText item)
        {
            base.OnCreated(item);
            item.onEndTween.AddListener(() => Despawn(item));
        }

        protected override void Reinitialize(string text,
                                             Vector3 startingPosition,
                                             float duration,
                                             Vector3 velocity,
                                             CritFloatingText item)
        {
            item.transform.position = startingPosition;
            item.label.text = text;
            item.canvasGroup.alpha = 1;
            item.duration = duration;
            item.velocity = velocity;

            item.ResetColor();
        }
    }
   
}
