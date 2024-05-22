using UnityEngine;
using System.Collections.Generic;
using System;
using UniRx;

[Serializable]
public class Stat
{
    [SerializeField] float baseValue;
    [SerializeField] public FloatReactiveProperty result = new();

    List<Modifier> adders = new();
    List<Modifier> multipliers = new();

    Dictionary<Modifier, IDisposable> disposables = new();

    public void AddAdder(Modifier adder)
    {
        adders.Add(adder);

        disposables.Add(adder, adder.Enabled.Subscribe(_ => Calculate()));

        Calculate();
    }
    public void RemoveAdder(Modifier adder)
    {
        adders.Remove(adder);

        disposables.Remove(adder, out IDisposable subscription);
        subscription.Dispose();

        Calculate();
    }

    public void AddMultiplier(Modifier multiplier)
    {
        multipliers.Add(multiplier);

        disposables.Add(multiplier, multiplier.Enabled.Subscribe(_ => Calculate()));

        Calculate();
    }
    public void RemoveMultiplier(Modifier multiplier)
    {
        multipliers.Remove(multiplier);

        disposables.Remove(multiplier, out IDisposable subscription);
        subscription.Dispose();

        Calculate();
    }
   
    public static implicit operator float(Stat stat)
    {
        return stat.result.Value;
    }

    public float Calculate()
    {
        float res = baseValue;

        foreach (var item in adders)
            if (item.Enabled.Value)
                res += item;


        foreach (var item in multipliers)
            if (item.Enabled.Value)
                res *= item;

        result.Value = res;

        return res;
    }
}
