using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public abstract class Ability : Talent, ITickable
{
    [SerializeField] protected bool isMagic;
    [SerializeField] protected float energyCost;
    [SerializeField] [HideInInspector] protected List<Field> cooldowns;

    [HideInInspector] public Volume cooldown = new();

    [Inject] Encounter encounter;
    [Inject] protected Character character;

    protected AbilityButton abilityButton;
    protected AbilityView abilityView;
    protected Stat speedStat;

    public void SubscribeToCheats(Cheats cheats)
    {
        cheats.noCooldown
            .SubToggle(cooldown.ObserveFull().WhereEqual(false),
                       _ => cooldown.Refill())
            .AddTo(abilityButton);
    }

    abstract protected void Use();

    virtual protected void ConcreteSubscribe()
    {
    }

    public void SubscribeView(Character character,
                              AbilityView abilityView)
    {
        this.character = character;
        this.abilityView = abilityView;
        this.abilityButton = abilityView.abilityButton;

        abilityView.Connect(this);
        abilityButton.Connect(this);

        ConcreteSubscribe();
    }

    public IObservable<bool> ObserveReadyToUse()
    {
        return
            Observable
            .CombineLatest(cooldown.ObserveFull(),
                           ObserveHaveEnoughEnergy(),
                           (isReady, isEnough) => isReady && isEnough);
    }

    public void Initialize()
    {
        speedStat = (isMagic) ? character.Stats.magicSpeed : character.Stats.speed;
        cooldown.ResizeAndRefill(cooldowns[Level]);
    }

    public IObservable<bool> ObserveHaveEnoughEnergy(float timeDelta = 1)
    {
        return
            character.mana
            .current
            .Select(current => (current >= energyCost * timeDelta));
    }

    protected void DrainEnergy(float energyCost, float deltaTime = 1)
    {
        character.mana.Subtract(energyCost * deltaTime);
    }

    protected void DrainEnergy(float deltaTime = 1)
    {
        character.mana.Subtract(energyCost * deltaTime);
    }

    public virtual void Tick()
    {
        if (!cooldown.IsFull)
        {
            cooldown.Add(Time.deltaTime * speedStat);
        }
    }

    public async UniTask UseAsync()
    {
        OnPress();

        {
            abilityView.OnPointerDown().Forget();

            await
                abilityButton.onPointerUp.OnInvokeAsync(
                    abilityButton.GetCancellationTokenOnDestroy());

            abilityView.OnPointerUp();
        }

        OnWheelZone(abilityView.wheel.GetZoneUnderCursor());

        OnEnd();

        cooldown.ResetToZero();
    }

    [Inject] void DefferedSubscribe_ZonesUpdate()
    {
        UniTask
            .WaitUntil(() => character?.Stats != null)
            .ContinueWith(() =>
            {
                character.onTargetSet.AsObservable()
                    .WhereNotNull()
                    .Subscribe(mob =>
                    {
                        Observable
                            .CombineLatest(character.Stats.ObserveWheelStatChanges(),
                                           mob.Stats.ObserveWheelStatChanges())
                            .TakeWhile(_ => mob.gameObject.activeSelf)
                            .Subscribe(_ =>
                            {
                                float
                                    missZone = character.GetMissChance(),
                                    critZone = character.GetCritChance(isMagic),
                                    damageZone = 1f - missZone - critZone;

                                abilityView.wheel.SetZones((Wheel.Zone.Miss, missZone),
                                                           (Wheel.Zone.Damage, damageZone),
                                                           (Wheel.Zone.Crit, critZone));
                            });
                    })
                    .AddTo(character);
            })
            .Forget();
    }

    protected abstract void OnPress();

    public void OnWheelZone(Wheel.Zone zone)
    {
        switch (zone)
        {
            case Wheel.Zone.Miss:
                OnMiss();
                break;
            case Wheel.Zone.Damage:
                OnDamage();
                break;
            case Wheel.Zone.Crit:
                OnCrit();
                break;
        }
    }

    protected virtual void OnMiss() {  }
    protected virtual void OnDamage() {  }
    protected virtual void OnCrit() {  }

    protected virtual void OnEnd() {  }
}
