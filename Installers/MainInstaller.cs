using System.Linq;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

public class MainInstaller : BaseInstaller
{
    [SerializeField] Character character;
    [Space]
    [SerializeField] Journey journey;
    [SerializeField] SplineMove splineMove;
    [Space]
    [SerializeField] SegmentedProgressBar arenaProgressBar;
    [Header("Instances")]
    [SerializeField] BattleTrigger battleTrigger;
    [SerializeField] Encounter encounter;
    [SerializeField] Land land;
    [SerializeField] Battle battle;
    [SerializeField] NominalParticles nominalParticles;
    [Header("Prefabs")]
    [SerializeField] Prop prefabProp;
    [SerializeField] Mob prefabMob;

    override public void InstallBindings()
    {
        base.InstallBindings();

        Container.Bind<Journey>().FromInstance(journey);
        Container.Bind<SplineMove>().FromInstance(splineMove);
        Container.Bind<BattleTrigger>().FromInstance(battleTrigger);
        Container.Bind<Encounter>().FromInstance(encounter);
        Container.Bind<Battle>().FromInstance(battle);

        Container
            .Bind<NominalParticles>()
            .FromInstance(nominalParticles)
            .AsSingle();


        Container.BindSOs<Talent>("SO/Talents/",
                                  "SO/Upgrades/Stats/");

        Container.BindSOs<Ability>("SO/Abilities/");

        Container
            .BindMemoryPool<Mob, Mob.Pool>()
            .FromComponentInNewPrefab(prefabMob);


        Container
            .BindMemoryPool<Prop, Prop.Pool>()
            .FromComponentInNewPrefab(prefabProp);


        // Container
        //     .Bind<SegmentedProgressBar>()
        //     .FromMethod(() => arenaProgressBar)
        //     .WhenInjectedInto<Journey>();

        // Container
        //     .BindMemoryPool<WeakPointView, WeakPointView.Pool>()
        //     .FromComponentInNewPrefab(prefabWeakPoint)
        //     .UnderTransform(canvas.transform);

        // Container
        //     .Bind<RuntimeAnimatorController>()
        //     .FromInstance(characterAnimatorController)
        //     .WhenInjectedInto<AttackInput>();

        // Container
        //     .BindMemoryPool<Ally, Ally.Pool>()
        //     .FromComponentInNewPrefab( prefabAlly )
        //     .AsTransient();

        Container
            .Bind<Character>()
            .FromInstance(character)
            .AsSingle();

        // Container
        //     .Bind(typeof(DamageModifier), typeof(IInitializable))
        //     .To(t => t.AllTypes().DerivingFrom<DamageModifier>())
        //     .AsTransient()
        //     .NonLazy();
    }
}
