using UnityEngine;
using System;
using Zenject;
using System.Collections.Generic;

public abstract class Talent : ScriptableObject
{
    [SerializeField] public Sprite sprite;
    [SerializeField] [HideInInspector] protected int maxLevel;
    [SerializeField] [HideInInspector] protected List<Field> cost = new();

    [HideInInspector] public Buyable<Level> buyableLevel;

    public int Level => (int)buyableLevel.ware.level.current.Value;

    protected abstract void OnLevelUp();

    public abstract IObservable<string> ObserveDescription();

    [Inject]
    public void RegisterWithSaveSystem(SaveSystem saveSystem)
    {
        saveSystem
            .MaybeRegister<float>(this,
                                  $"{name}:level",
                                  () => buyableLevel.ware.level.current.Value,
                                  (val) => buyableLevel.ware.SetLevel((int)val));
    }

    [Inject]
    protected void InitializeBuyableLevel(Vault vault)
    {
        Talent_SubInitialize();

        Price price = new Price(vault.gold);
        Level level = new Level(l => price.cost.Value = cost[Level]);

        buyableLevel = new Buyable<Level>(level,
                                          level => level.Up(),
                                          price);

        level.SetMaximum(maxLevel);
        level.SetLevel(0);
    }

    protected virtual void Talent_SubInitialize()
    {

    }

    [Inject] protected void ConnectToView(TalentView talentView)
    {
        talentView.ConnectBase(this);
    }

    [Serializable]
    public struct Field
    {
        public int intValue;

        static public implicit operator int(Field field)
        {
            return field.intValue;
        }
    }
}
