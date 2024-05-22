using System;
using UnityEngine;
using UnityEngine.Splines;
using UniRx;

public abstract class MySplineComponent : MonoBehaviour
{
    [SerializeField] protected float zAngleCorrection;

    public ReactiveProperty<SplineContainer> targetSpline = new();

    public void EvaluatePositionAndRotation(float t, out Vector3 position, out Quaternion rotation)
    {
        position = targetSpline.Value.EvaluatePosition(t);

        var axisRemapRotation = Quaternion.Inverse(Quaternion.LookRotation(Vector3.forward, Vector3.up));
        var forward = Vector3.Normalize(targetSpline.Value.EvaluateTangent(t));
        var up = targetSpline.Value.EvaluateUpVector(t);

        rotation =
            Quaternion.LookRotation(forward, up)
            * Quaternion.Euler(0, 0, zAngleCorrection)
            * axisRemapRotation;
    }
}
