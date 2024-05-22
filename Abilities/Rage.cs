using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Rage", menuName = "SO/Abilities/Rage")]
public class Rage : Ability
{
    [SerializeField] [HideInInspector] List<Field> damageMultipliers;
    [SerializeField] [HideInInspector] List<Field> durations;

    Buff attackBuff = new();
    Modifier strengthModifier;

    protected override void ConcreteSubscribe()
    {
        base.ConcreteSubscribe();

        strengthModifier = new Modifier();
        attackBuff.AddModifier(strengthModifier);

    }


    protected override void OnPress()
    {

    }
    protected override void Use()
    {
        attackBuff.StartBuff(character);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Attack Buff");
    }

    protected override void OnLevelUp()
    {
        strengthModifier.Value = damageMultipliers[Level];
        attackBuff.duration = durations[Level];
    }
}
