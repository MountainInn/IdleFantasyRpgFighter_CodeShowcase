using UnityEngine;
using Zenject;
using UniRx;
using System;
using System.Linq;

public class FloatingTextSpawner : MonoBehaviour
{
    [SerializeField] protected float duration;
    [SerializeField] protected Vector3 velocity;
    [SerializeField] protected Vector3 offset;
    [SerializeField] protected int anglePerActive = -4;
    [SerializeField] protected int maxCount = 24;

    [Inject] protected FloatingText.Pool floatingTextPool;
    [Inject] protected CritFloatingText.Pool critFloatingTextPool;

    protected int active;
    protected int halfMaxCount => maxCount / 2;

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
       
        for (int i = 0; i < maxCount; i++)
        {
            int j = i - halfMaxCount;

            Vector3 privateOffset = GetOffset(j);
            Vector3 privateVelocity = GetVelocity(j);

            Vector3 spawnPos = transform.position + privateOffset;

            Gizmos.DrawLine(spawnPos,
                            spawnPos + privateVelocity * duration);
        }
    }

    public void FloatDamage(DamageArgs args)
    {
        string text = args.damage.ToString("F1");

        int i = active - halfMaxCount;

        Vector3 privateVelocity = GetVelocity(i);

        Vector3 privateOffset = GetOffset(i);

        FloatingTextBase floatingText =
            (args.isCrit)?
            critFloatingTextPool.Spawn(text,
                                       transform.position + privateOffset,
                                       duration,
                                       privateVelocity)
            :
            floatingTextPool.Spawn(text,
                                   transform.position + privateOffset,
                                   duration,
                                   privateVelocity)
            ;

        floatingText.StartTween();

        active++;
        active %= maxCount;
    }

    public void Float(string text)
    {
        int i = active - halfMaxCount;

        Vector3 privateVelocity = GetVelocity(i);

        Vector3 privateOffset = GetOffset(i);

        var floatingText =
            floatingTextPool.Spawn(text,
                                   transform.position + privateOffset,
                                   duration,
                                   privateVelocity);

        Color textColor = text.Contains('-') ? Color.red : Color.green;

        floatingText.SetColor(textColor);
        floatingText.StartTween();

        active++;
        active %= maxCount;
    }

    protected Vector3 GetOffset(int i)
    {
        return i * offset + UnityEngine.Random.onUnitSphere;
    }

    protected Vector3 GetVelocity(int i)
    {
        int angle = i * anglePerActive;

        return Quaternion.Euler(0, 0, angle) * velocity;
    }
}
