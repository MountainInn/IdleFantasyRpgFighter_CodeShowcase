using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AbilityButtonsInstaller : MonoInstaller
{
    [SerializeField] RectTransform abilitiesParent;
    [SerializeField] AbilityView abilityViewPrefab;

    override public void InstallBindings()
    {
        Container
            .BindView(abilityViewPrefab, abilitiesParent);
    }
}
