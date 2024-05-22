using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;
using UniRx;

public class MobSpawner : Spawner
{
    [Inject] Journey journey;
    [Inject] Mob.Pool pool;

    protected override void Construct()
    {
        splineMove
            .targetSpline
            .WhereNotNull()
            .Subscribe(Populate)
            .AddTo(this);
    }

    protected override void Spawn(float3 point, Quaternion rotation)
    {
        MobStatsSO stats = journey.GetNextMob();

        Mob mob = pool.Spawn(point);

        mob.transform.rotation = rotation;

        stats.Apply(mob);
    }
}
