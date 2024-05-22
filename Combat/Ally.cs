using Zenject;
using UniRx;
using UnityEngine;

public class Ally : AnimatorCombatant
{
    protected MobStatsSO mobStats;
    public MobStatsSO MobStats => mobStats;

    protected bool mobCanAttack;

    protected new void Awake()
    {
        base.Awake();

        SubscribeCanAttack();
        SubscribeToAttackTimerFull();
    }

    protected void SubscribeToAttackTimerFull()
    {
        attackTimer.ObserveFull()
            .WhereEqual(true)
            .Subscribe(_ => combatantAnimator.SetTrigger(basicAttackTriggerId))
            .AddTo(this);
    }

    protected void SubscribeCanAttack()
    {
        mobCanAttack = true;
        onDie.AddListener(() => mobCanAttack = false);
        onRespawn.AddListener(() => mobCanAttack = true);
    }

    public void Update()
    {
        if (mobCanAttack && CanContinueBattle())
            AttackTimerTick(Time.deltaTime);
    }

    public class Pool : MonoMemoryPool<Ally>
    {
       
    }
}
