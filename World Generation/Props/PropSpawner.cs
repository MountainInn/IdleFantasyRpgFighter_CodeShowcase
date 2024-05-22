using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;
using UniRx;
using System.Collections.Generic;
using System.Linq;

public class PropSpawner : Spawner
{
    [SerializeField] float minOrthogonalOffset, maxOrthogonalOffset;

    [Inject] Character character;
    [Inject] Prop.Pool pool;

    protected override void Construct()
    {
        splineMove
            .targetSpline
            .WhereNotNull()
            .Subscribe(Populate)
            .AddTo(this);
    }

    protected override void TransformPositionAndRotation(ref Vector3 position, ref Quaternion rotation)
    {
        float orthogonalOffset =
            UnityEngine.Random.Range(minOrthogonalOffset,
                                     maxOrthogonalOffset)
            *
            ((UnityEngine.Random.value < .5f) ? -1 : 1);

        Vector3 orthogonal = rotation * new Vector3(orthogonalOffset, 0, 0);

        position = position + orthogonal;
    }


    protected override void Spawn(float3 point, Quaternion rotation)
    {
        Prop prop = pool.Spawn(point);

        prop
            .GetComponent<FaceTarget>()
            .SetTarget(character.transform);

    }
}
