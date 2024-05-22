using UnityEngine;
using DG.Tweening;

static public class TweenExt
{
    static public Tween CreateWindowHideTween(RectTransform rectTransform, float widthMultiplier)
    {
        float hiddenInventoryX = rectTransform.sizeDelta.x * widthMultiplier;

        return
            rectTransform
            .DOLocalMoveX(hiddenInventoryX, 0.25f)
            .SetUpdate(isIndependentUpdate: true)
            .SetAutoKill(false)
            .SetEase(Ease.InQuad)
            .Pause();
    }
}
