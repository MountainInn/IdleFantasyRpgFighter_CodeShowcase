using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;
using UniRx;
using System.Collections.Generic;
using System.Linq;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField] protected float minDistance, maxDistance;
    [SerializeField] protected float verticalPadding;

    [Inject] protected GameSettings gameSettings;
    [Inject] protected SplineMove splineMove;
    [Inject] protected Treadmill treadmill;
    [Inject] protected RoadCaster roadCaster;

    protected List<float> distances = new();

    [Inject] protected abstract void Construct();


    public void Populate(SplineContainer roadSpline)
    {
        var positionsAndRotations = CalculateSpawnPositionsAndRotations(roadSpline);

        positionsAndRotations
            .Map(tuple => Spawn(tuple.position, tuple.rotation));
    }

    protected IEnumerable<(float3 position, Quaternion rotation)> CalculateSpawnPositionsAndRotations(SplineContainer roadSpline)
    {
        distances.Clear();

        float splineLength = roadSpline.Spline.GetLength();
        float coveredLength = 0;

        while ((splineLength - coveredLength) > maxDistance)
        {
            float nextSpawnLength = RollSpawnPoint();

            coveredLength += nextSpawnLength;

            if (coveredLength < splineLength)
                distances.Add(coveredLength);
        }

        return
            distances
            .Select(length =>
            {
                splineMove.EvaluatePositionAndRotation(length / splineLength,
                                                       out Vector3 evaluatedPosition,
                                                       out Quaternion rotation);

                Vector3 absPosition = treadmill.ToAbsolutePosition(evaluatedPosition);

                TransformPositionAndRotation(ref absPosition, ref rotation);

                float3 castPosition =
                    roadCaster.CastDown(treadmill.GetAbsolutePosition(roadSpline.transform),
                                        absPosition,
                                        verticalPadding);


                return (castPosition, rotation);
            });
    }

    protected virtual void TransformPositionAndRotation(ref Vector3 position, ref Quaternion rotation)
    {
       
    }

    protected float RollSpawnPoint()
    {
        return UnityEngine.Random.Range(minDistance, maxDistance);
    }

    protected abstract void Spawn(float3 point, Quaternion rotation);
}
