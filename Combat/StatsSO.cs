using System;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsSO", menuName = "SO/StatsSO")]
public class StatsSO : ScriptableObject
{
    [SerializeField] public Stat health;
    [SerializeField] public Stat mana;
    [SerializeField] public Stat stamina;
    [Space]
    [SerializeField] public Stat strength;
    [SerializeField] public Stat intelligence;
    [Space]
    [SerializeField] public Stat agility;
    [SerializeField] public Stat perception;
    [SerializeField] public Stat luck;
    [Space]
    [SerializeField] public Stat speed;
    [SerializeField] public Stat magicSpeed;
    [Space]
    [SerializeField] public Stat critChance_Phys;
    [SerializeField] public Stat critChance_Mag;
    [SerializeField] public Stat critMult;
    [Space]
    [SerializeField] public Stat attackTimer;

    public void Apply(Combatant combatant)
    {
        var stats = combatant.Stats = this;

        combatant.health.ResizeAndRefill(health);
        combatant.mana.ResizeAndRefill(stamina);

        combatant.attackTimer.ResetToZero();
        combatant.attackTimer.Resize(attackTimer);

        combatant.onStatsApplied?.Invoke();
    }

    public void CalculateAll()
    {
        health          .Calculate();
        mana            .Calculate();
        stamina         .Calculate();
        strength        .Calculate();
        intelligence    .Calculate();
        perception      .Calculate();
        luck            .Calculate();
        agility         .Calculate();
        speed           .Calculate();
        magicSpeed      .Calculate();
        critChance_Phys .Calculate();
        critChance_Mag  .Calculate();
        critMult        .Calculate();
        attackTimer     .Calculate();
    }

    public IObservable<Unit> ObserveWheelStatChanges()
    {
        return
            Observable
            .CombineLatest(agility.result,
                           perception.result,
                           critChance_Phys.result,
                           critChance_Mag.result)
            .Select(_ => Unit.Default);
    }
}
