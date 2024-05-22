using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;
using UniRx;

public class FaceTarget : MonoBehaviour
{
    [SerializeField] Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    void Update()
    {
        if (target == null)
            return;
       
        Vector3 relativePos = target.position - transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        transform.eulerAngles = rotation.eulerAngles.WithX(0).WithZ(0);
    }
}
