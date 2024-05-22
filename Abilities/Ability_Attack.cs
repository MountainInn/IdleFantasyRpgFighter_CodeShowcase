using UniRx;
using UnityEngine;

abstract public class Ability_Attack : Ability
{
    [SerializeField] public string attackAnimationTrigger;

    public DamageArgs lastCreatedArgs;

    protected override void ConcreteSubscribe()
    {
        Subscribe_EnergyDrain();
    }

    void Subscribe_EnergyDrain()
    {
        character
            .preAttack
            .AsObservable()
            .Subscribe(args =>
            {
                if (args == lastCreatedArgs)
                    DrainEnergy();
            })
            .AddTo(abilityButton);
    }


    protected override void OnPress()
    {
        lastCreatedArgs = character.CreateDamage(isMagic);
        lastCreatedArgs.animationTrigger = attackAnimationTrigger;
    }

    protected override void OnMiss()
    {
        lastCreatedArgs.isMissed = true;
    }

    protected override void OnDamage()
    {

    }

    protected override void OnCrit()
    {
        lastCreatedArgs.damage *= character.Stats.critMult;
        lastCreatedArgs.isCrit = true;
    }

    protected override void OnEnd()
    {
        lastCreatedArgs.defender = character.target;

        if (lastCreatedArgs.defender != null)
            character.PushAttack(lastCreatedArgs);
    }
}
