using System;
using UniRx;
using UniRx.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class Level : Buyable<Level>.IWare
{
    public Volume level = new();

    protected List<Action<int>> statCalculations;


    public Level(int maximumLevel, Action<int> statsCalculation)
        : this(statsCalculation)
    {
        SetMaximum(maximumLevel);
    }

    public Level(Action<int> statsCalculation)
    {
        statCalculations = new List<Action<int>>(){
            statsCalculation
        };
    }

    public int ToInt()
    {
        return (int)level.current.Value;
    }

    public void AddCalculation(params Action<int>[] statCalculations)
    {
        this.statCalculations.AddRange(statCalculations);
    }

    public void Up()
    {
        SetLevel((int)level.current.Value + 1);
        Debug.Log($"Up {level}");
    }

    public void SetMaximum(int maximumLevel)
    {
        level.Resize(maximumLevel);
    }

    public void SetLevel(int level)
    {
        this.level.current.Value = level;

        foreach (var item in statCalculations)
        {
            item.Invoke(level);
        }
    }

    public IObservable<bool> ObserveIsAtMaxLevel()
    {
        return level.ObserveFull();
    }

    public IObservable<bool> ObserveIsAffordable()
    {
        return
            ObserveIsAtMaxLevel()
            .Select(b => !b);
    }
}
