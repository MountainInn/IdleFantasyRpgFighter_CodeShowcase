using UnityEngine;
using DG.Tweening;
using Zenject;

public class AttackBonusVfx : MonoBehaviour
{
    [SerializeField] float fadeDuration;
    [SerializeField] MeshRenderer meshRenderer;

    void Start()
    {
        HideBonus();
    }

    public void ShowBonus()
    {
        meshRenderer
            .material
            .DOFade(1f, "_TintColor", fadeDuration);
    }

    public void HideBonus()
    {
        meshRenderer
            .material
            .DOFade(0f, "_TintColor", fadeDuration);
    }
}
