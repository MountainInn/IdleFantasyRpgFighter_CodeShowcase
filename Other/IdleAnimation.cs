using UnityEngine;
using DG.Tweening;
using System.Collections;

public class IdleAnimation : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] Mob mob;

    void Start()
    {
        StartBreathing();

        StartShuffling();

        StartJumping();
    }

    void StartJumping()
    {
        StartCoroutine(JumpingCoroutine());

        IEnumerator JumpingCoroutine()
        {
            float posY = target.position.y;
            float duration = 3.5f * 2f;

            while (mob.IsAlive)
            {
                yield return
                    target
                    .DOMoveY(posY + 0.15f, duration)
                    .SetEase(Ease.InOutSine)
                    .WaitForKill();

                yield return
                    target
                    .DOMoveY(posY - 0.15f, duration)
                    .SetEase(Ease.InOutSine)
                    .WaitForKill();
            }
        }
    }

    void StartShuffling()
    {
        StartCoroutine(ShufflingCoroutine());

        IEnumerator ShufflingCoroutine()
        {
            float posX = target.position.x;
            float duration = 6 * 2f;

            while (mob.IsAlive)
            {
                yield return
                    target
                    .DOMoveX(posX + 0.1f, duration)
                    .SetEase(Ease.InOutBack)
                    .WaitForKill();

                yield return
                    target
                    .DOMoveX(posX - 0.1f, duration)
                    .SetEase(Ease.InOutBack)
                    .WaitForKill();
            }
        }
    }

    void StartBreathing()
    {
        StartCoroutine(BreathingCoroutine());

        IEnumerator BreathingCoroutine()
        {
            float scaleY = target.localScale.y;
            float duration = 2 * 2f;

            while (mob.IsAlive)
            {
                yield return
                    target
                    .DOScaleY(scaleY * 1.02f, duration)
                    .SetEase(Ease.OutQuad)
                    .WaitForKill();

                yield return
                    target
                    .DOScaleY(scaleY * 0.98f, duration)
                    .SetEase(Ease.OutQuad)
                    .WaitForKill();
            }
        }
    }
}
