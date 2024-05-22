using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PowerAttack", menuName = "SO/Abilities/PowerAttack")]
public class PowerAttack : Ability_Attack
{
    [SerializeField] [HideInInspector] List<Field> damages;

    float damage;

    protected override void Use()
    {
        lastCreatedArgs = character.CreateDamage();
        lastCreatedArgs.damage = damage;
        lastCreatedArgs.isPower = true;
        lastCreatedArgs.animationTrigger = attackAnimationTrigger;

        character.PushAttack(lastCreatedArgs);
    }


    protected override void OnPress()
    {

    }
    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Power Attack");
    }

    protected override void OnLevelUp()
    {
        damage = damages[Level];
    }
}
