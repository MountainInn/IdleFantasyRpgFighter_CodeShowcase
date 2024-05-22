using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Fortification", menuName = "SO/Abilities/Fortification")]
public class Fortification : Ability
{
    [SerializeField] [HideInInspector] List<Field> blockMultipliers;
    [SerializeField] [HideInInspector] List<Field> durations;

    Buff buff = new();
    Modifier blockValueMultiplier = new();
    Modifier energyDrainMult = new();

    [Inject] Block block;

    protected override void ConcreteSubscribe()
    {
        base.ConcreteSubscribe();

        buff.AddModifier(blockValueMultiplier);
        buff.AddModifier(energyDrainMult);

        energyDrainMult.Value = 0;
    }

    protected override void OnPress()
    {

    }

    protected override void Use()
    {
        buff.StartBuff(abilityButton);
    }

    public override IObservable<string> ObserveDescription()
    {
        return Observable.Return("Fortification");
    }

    protected override void OnLevelUp()
    {
        blockValueMultiplier.Value = blockMultipliers[Level];
        buff.duration = durations[Level];
    }
}
