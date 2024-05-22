using UnityEngine;
using UnityEngine.Events;
using Zenject;
using UniRx;
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;

public class Battle : MonoBehaviour
{
    [Inject] Canvas canvas;
    [Inject] Character character;

    [Inject]
    public void Construct(DiContainer Container,
                          Cheats cheats,
                          List<Ability> abilities)
    {
        abilities
            .Map(a =>
            {
                var abilityView = Container.Resolve<AbilityView>();
                a.SubscribeView(character, abilityView);
                a.SubscribeToCheats(cheats);
                a.Initialize();
            });
    }

    // [Inject]
    // public void SubscribeBlock(Block block, BlockVfx blockVfx, AttackBonusVfx attackBonusVfx)
    // {
    //     block.Subscribe(blockVfx, attackBonusVfx);
    // }

}
