using UnityEngine;
using DG.Tweening;
using Zenject;

public class BlockVfx : MonoBehaviour
{
    [SerializeField] Color blockColor = Color.blue;
    [SerializeField] Color parryColor = Color.red;
    [SerializeField] Color counterAttackColor = Color.yellow;
    [Space]
    [SerializeField] float blockFadeDuration = .1f;
    [SerializeField] float counterAttackFlashDuration = .25f;
    [Space]
    [SerializeField] MeshRenderer blockRenderer;

    [Inject] Character character;

    void Start()
    {
        HideBlock();
    }

    public void ShowBlock()
    {
        blockRenderer
            .material
            .DOFade(1f, "_TintColor", blockFadeDuration);

        character.combatantAnimator.SetBool("block", true);
    }

    public void HideBlock()
    {
        blockRenderer
            .material
            .DOFade(0f, "_TintColor", blockFadeDuration);

        character.combatantAnimator.SetBool("block", false);
    }

    public void ShowParry()
    {
        blockRenderer
            .material
            .DOColor(parryColor, "_TintColor", blockFadeDuration);
    }

    public void HideParry()
    {
        blockRenderer
            .material
            .DOColor(blockColor, "_TintColor", blockFadeDuration);
    }

    public void ShowCounterAttack()
    {
        DOTween
            .Sequence()
            .Append(blockRenderer
                    .material
                    .DOColor(counterAttackColor, "_TintColor", counterAttackFlashDuration)
                    .SetEase(Ease.Flash, 1))
            .Append(blockRenderer
                    .material
                    .DOColor(blockColor, "_TintColor", counterAttackFlashDuration)
                    .SetEase(Ease.Flash, 1));
    }
}
