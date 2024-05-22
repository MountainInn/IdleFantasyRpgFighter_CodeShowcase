using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;
using Zenject;
using System;
using System.Collections.Generic;

public abstract class AnimatorCombatant : Combatant
{
    [SerializeField] public Animator combatantAnimator;
    [SerializeField] protected string attackTriggerName = "attackTrigger";
    [SerializeField] public UnityEvent afterDeathAnimation;
    [SerializeField] public UnityEvent onAttackAnimEvent;
    [SerializeField] public UnityEvent<DamageArgs> onAttackPushed;

    [HideInInspector] public ObservableStateMachineTrigger ObserveStateMachine
    {
        get {
            if (_ObserveStateMachine == null)
                InitObserveStateMachine();

            return _ObserveStateMachine;
        }

        protected set => _ObserveStateMachine = value;
    }
    [SerializeField] public  ObservableStateMachineTrigger _ObserveStateMachine;

    protected int basicAttackTriggerId;

    public System.IObservable<bool> ObserveIsPlaying()
    {
        return
            Observable
            .CombineLatest(ObserveStateMachine.OnStateEnterAsObservable(),
                           ObserveStateMachine.OnStateExitAsObservable(),
                           (enter, exit) => enter.Equals(exit));
    }

    protected bool isPlaying;

    public void Awake()
    {
        InitObserveStateMachine();

        ObserveIsPlaying()
            .Subscribe(isPlaying =>
            {
                this.isPlaying = isPlaying;
            })
            .AddTo(this);

        basicAttackTriggerId = Animator.StringToHash(attackTriggerName);

        onDie.AddListener(() => combatantAnimator.SetBool("death", true));
        onRespawn.AddListener(() => combatantAnimator.SetBool("death", false));
    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {
        combatantAnimator.runtimeAnimatorController = controller;
        InitObserveStateMachine();
    }

    void InitObserveStateMachine()
    {
        ObserveStateMachine = combatantAnimator.GetBehaviour<ObservableStateMachineTrigger>();
    }


    DamageArgs lastAttack;

    public void PushAttack()
    {
        DamageArgs damageArgs = CreateDamage();
        damageArgs.animationTrigger = attackTriggerName;
       
        PushAttack(damageArgs);
    }
    public void PushAttack(DamageArgs attack)
    {
        if (!CanContinueBattle())
            return;

        onAttackPushed?.Invoke(attack);

        combatantAnimator.SetTrigger(attack.animationTrigger);
        lastAttack = attack;
    }

    public void InflictDamage_OnAnimEvent()
    {
        InflictDamage(lastAttack ?? CreateDamage());

        onAttackAnimEvent?.Invoke();
    }

    public void Die()
    {
        afterDeathAnimation?.Invoke();
    }
}
