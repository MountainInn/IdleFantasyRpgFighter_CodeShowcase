using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using System.Collections.Generic;
using System;

public class Character : AnimatorCombatant
{
    [SerializeField] CharacterStatsSO characterStatsSO;
    [SerializeField] SphereCollider goldCollectionField;

    List<ITickable> tickables = new();

    [Inject] DiContainer Container;
    [Inject] void Construct()
    {
        mana = new();

        characterStatsSO . ToStats() . Apply(this);
    }

    public SphereCollider GetGoldCollectionField()
    {
        return goldCollectionField;
    }
    
    [Inject] void SubscribeToNextMob(Mob.Pool mobPool)
    {
        mobPool.mobs.next
            .Subscribe(SetTarget)
            .AddTo(this);
    }
   

    [Inject] void SubscribeToCheats(Cheats cheats)
    {
        cheats.oneShotMob
            .SubToggle(preAttack.AsObservable(),
                       args =>
                       args.damage = args.defender.health.maximum.Value)
            .AddTo(this);

        cheats.oneShotCharacter
            .SubToggle(preTakeDamage.AsObservable(),
                       args =>
                       args.damage = args.defender.health.maximum.Value)
            .AddTo(this);

        cheats.godMode
            .Subscribe(toggle =>
            {
                if (toggle)
                    health.ResizeAndRefill(int.MaxValue);
                else
                    health.ResizeAndRefill(characterStatsSO.health);
            })
            .AddTo(this);
    }

    [Inject] public void SubView(CharacterView characterView)
    {
        characterView.Subscribe(this);
    }

    public override void InflictDamage(DamageArgs args)
    {
        if (args?.defender == null || !args.defender.IsAlive)
            return;

        preAttack?.Invoke(args);

        args.defender.TakeDamage(args);

        postAttack?.Invoke(args);

        if (!args.defender.IsAlive)
        {
            onKill?.Invoke(args.defender);

            args.defender.onDie?.Invoke();
        }
    }

    public void SetTarget(Combatant target)
    {
        this.target = target;
        onTargetSet?.Invoke(target);
    }
}
