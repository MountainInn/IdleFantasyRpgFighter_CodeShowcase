using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class HitAnimation : MonoBehaviour
{
    [SerializeField] Image image;

    public void Animate()
    {
        StartCoroutine(TweenCoroutine());

        IEnumerator TweenCoroutine()
        {
            yield return
                image
                .DOFade(.1f, .15f)
                .SetEase(Ease.InSine)
                .WaitForKill();

            yield return
                image
                .DOFade(0f, .15f)
                .SetEase(Ease.InSine)
                .WaitForKill();

        }
    }
}
