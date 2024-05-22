using System;
using UnityEngine;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using Zenject;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

public class Buff
{
    public float duration;
    public BoolReactiveProperty enabled {get; protected set;} = new();

    List<Modifier> modifiers = new();

    float t;

    public Buff(params Modifier[] modifiers)
    {
        this.modifiers = modifiers.ToList();
    }

    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
    }

    public async UniTask StartBuff<T>(T holder)
        where T : Component
    {
        if (enabled.Value)
        {
            t = 0f;
            return;
        }

        enabled.Value = true;
        foreach (var item in modifiers)
            item.Enabled.Value = enabled.Value;

        for (t = 0f; t < duration; t += Time.deltaTime)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        enabled.Value = false;
        foreach (var item in modifiers)
            item.Enabled.Value = enabled.Value;
    }
}
