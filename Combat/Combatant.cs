using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

abstract public class Combatant : MonoBehaviour
{
    [SerializeField] public Volume health;
    [SerializeField] public Volume stamina;
    [SerializeField] public Volume mana;
    [SerializeField] public Volume attackTimer = new();
    [Space]
    [SerializeField] public UnityEvent onDie;
    [SerializeField] public UnityEvent onRespawn;
    [SerializeField] public UnityEvent<Combatant> onKill;
    [SerializeField] public UnityEvent onStatsApplied;
    [Space]
    [SerializeField] public DropList dropList;

    [HideInInspector] [SerializeField] public StatsSO Stats;

    [InjectOptional] public Combatant target;
    [SerializeField] public UnityEvent<Combatant> onTargetSet;

    [HideInInspector]
    public UnityEvent<DamageArgs>
        preAttack,
        preTakeDamage,
        postTakeDamage,
        postAttack,

        onMiss;

    public BoolReactiveProperty reactiveIsAlive {get; protected set;} = new();

    void Awake()
    {
        health
            .ObserveEmpty()
            .Select(b => !b)
            .Subscribe(b => reactiveIsAlive.Value = b)
            .AddTo(this);
    }

    protected void OnEnable()
    {
        health.Refill();
    }

    public float GetMissChance()
    {
        return GetMissChance(target);
    }

    public float GetMissChance(Combatant target)
    {
        return 1f - GetHitChance(target);
    }

    public virtual float GetHitChance(Combatant target)
    {
        float hitChance =
            Stats.perception
            /
            target.Stats.agility * 2;

        hitChance = Mathf.Clamp01(hitChance);

        return hitChance;
    }

    public virtual float GetCritChance(bool isMagic)
    {
        float critChance =
            isMagic
            ? Stats.critChance_Mag
            : Stats.critChance_Phys;

        return critChance;
    }


    public bool AttackTimerTick(float delta)
    {
        attackTimer.Add(delta);

        bool isFull = attackTimer.IsFull;

        if (isFull)
            attackTimer.ResetToZero();

        return isFull;
    }

    public void InflictDamage(Combatant defender)
    {
        InflictDamage(defender, Stats.strength);
    }

    public void InflictDamage(Combatant defender, float damage)
    {
        DamageArgs args = CreateDamage(defender, damage);

        InflictDamage(args);
    }

    public virtual void InflictDamage(DamageArgs args)
    {
        if (!args.defender.IsAlive)
            return;

        preAttack?.Invoke(args);

        /// Dodge Roll
        {
            float hitChance =
                args.attacker.Stats.perception
                /
                args.defender.Stats.agility * 2;

            if (UnityEngine.Random.value > hitChance)
            {
                onMiss?.Invoke(args);
                args.isMissed = true;
                args.damage = 0;
                return;
            }
        }
        /// Crit Roll
        {
            float critChance =
                args.isMagic
                ? args.attacker.Stats.critChance_Mag
                : args.attacker.Stats.critChance_Phys;

            if (UnityEngine.Random.value <= critChance)
            {
                args.isCrit = true;
                args.damage *= args.attacker.Stats.critMult;
            }
        }

        args.defender.TakeDamage(args);

        postAttack?.Invoke(args);

        if (!args.defender.IsAlive)
        {
            onKill?.Invoke(args.defender);

            args.defender.onDie?.Invoke();
        }
    }

    public DamageArgs CreateDamage()
    {
        return CreateDamage(target, Stats.strength);
    }

    public DamageArgs CreateDamage(bool isMagic)
    {
        DamageArgs args;

        if (isMagic)
        {
            args = CreateDamage(target, Stats.intelligence);
            args.isMagic = true;
        }
        else
        {
            args = CreateDamage(target, Stats.strength);
        }

        return args;
    }
    protected DamageArgs CreateDamage(Combatant defender, float damage)
    {
        return new DamageArgs()
        {
            attacker = this,
                defender = defender,
                damage = damage
                };
    }

    public void TakeDamage(DamageArgs args)
    {
        if (args.isMissed)
            return;

        preTakeDamage?.Invoke(args);

        health.Subtract(args.damage);

        postTakeDamage?.Invoke(args);
    }

    public void Respawn()
    {
        health.Refill();
        attackTimer.ResetToZero();

        onRespawn?.Invoke();
    }

    protected bool CanContinueBattle()
    {
        return IsAlive && target.IsAlive;
    }

    public bool IsAlive => health.current.Value > 0;
}
