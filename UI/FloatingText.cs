using UnityEngine;
using Zenject;
using UnityEngine.UI;

public class FloatingText : FloatingTextBase
{
    public class Pool : MonoMemoryPool<string, Vector3, float, Vector3, FloatingText>
    {
        protected override void OnCreated(FloatingText item)
        {
            base.OnCreated(item);
            item.onEndTween.AddListener(() => Despawn(item));
        }

        protected override void Reinitialize(string text,
                                             Vector3 startingPosition,
                                             float duration,
                                             Vector3 velocity,
                                             FloatingText item)
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
