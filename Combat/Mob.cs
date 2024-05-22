using UnityEngine;
using Zenject;
using UniRx;
using System.Collections.Generic;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Linq;

[RequireComponent(typeof(TrashTrigger))]
public class Mob : AnimatorCombatant
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public MobStatsSO startingMobStats;
    [Space]
    [SerializeField] public UnityEvent<IEnumerable<DropList.Entry>> onDropLoot;
    [SerializeField] public NominalParticles nominalParticles;


    [Inject] GameSettings gameSettings;
    [Inject] void SubscribeToCheats(Cheats cheats)
    {
        cheats.mobOneSecondAttackTimer
            .SubToggle(onStatsApplied.AsObservable(),
                       _ => attackTimer.Resize(1))
            .AddTo(this);

        cheats.trainingDummy
            .SubToggle(onStatsApplied.AsObservable(),
                       _ => health.ResizeAndRefill(int.MaxValue))
            .AddTo(this);

        onStatsApplied
            .AsObservable()
            .Take(1)
            .Subscribe(_ =>
            {
                cheats.mobOneSecondAttackTimer
                    .Subscribe(toggle => attackTimer.Resize(toggle ? 1 : Stats.attackTimer))
                    .AddTo(this);

                cheats.trainingDummy
                    .Subscribe(toggle => health.ResizeAndRefill(toggle ? int.MaxValue : Stats.health))
                    .AddTo(this);
            })
            .AddTo(this);
    }
    [Inject] void SubscribeFloatingText(FloatingTextSpawner takeDamagFloatingTextSpawner)
    {
        postTakeDamage
            .AsObservable()
            .Subscribe(args =>
            {
                takeDamagFloatingTextSpawner?.FloatDamage(args);
            })
            .AddTo(this);
    }
    [Inject] void SubscribeToDPSMeter(DPSMeter dpsMeter, DPSMeterView dpsView)
    {
        dpsMeter
            .ObserveDPS(this)
            .Subscribe(dpsView.SetText)
            .AddTo(this);
    }
    [Inject] void SubscribeToLootManager(LootManager lootManager)
    {
        lootManager.SubscribeForProps(onDropLoot, nominalParticles, gameObject);
    }

    protected new void Awake()
    {
        base.Awake();

        onDie.AddListener(() => RollLoot());

        Respawn();
    }

    void RollLoot()
    {
        var loot = dropList.Roll();

        if (loot.Any())
            onDropLoot?.Invoke(loot);
    }


    public class Pool : PoolOnTreadmill<Mob>
    {
        public ReactiveQueue<Mob> mobs = new();

        protected override void OnSpawned(Mob item)
        {
            base.OnSpawned(item);

            UniTask
                .WaitUntil(() => item.Stats != null)
                .ContinueWith(() => mobs.Enqueue(item))
                .Forget();
        }

        protected override void OnDespawned(Mob item)
        {
            base.OnDespawned(item);

            mobs.Dequeue();
        }

        protected override void OnCreated(Mob item)
        {
            base.OnCreated(item);

            item.gameObject
                .AddComponent<TrashTrigger>()
                .onEnterCleaner += () => Despawn(item);
        }
    }
}
