using System;
using UnityEngine;
using UniRx;

[CreateAssetMenu(fileName = "GameSettings", menuName = "SO/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Wheel")]
    public float cursorRotationSpeed = 30;
    [Header("World Extender")]
    public int worldSize = 1;
    [Header("Land")]
    public FloatReactiveProperty landSize = new(200);
    [Header("Roads")]
    public float roadMinimalTurnAngle = 30;
    public float roadMaximumTurnAngle = 40;
    public float roadSegmentLength = 100;
    public float roadTangentLength = 30;
    public float roadSpawnHeight = 500;
    [Header("Character")]
    public float movementSpeed = 10;
    [Header("DPS Meter")]
    public float dpsMeterSampleInterval = 10;
    [Header("Loot Manager")]
    public float intervalBetweenDrops = 0.15f;
    public int maxParticleCount = 8;
    [Header("Save System")]
    public double autoSaveInterval = 20;
    [Header("Other")]
    public FloatReactiveProperty globalTimeInterval = new(5);

    public void SubscribeToTimer<T>(IDisposable timerSubscription,
                                    Component holder,
                                    IObservable<T> takeUntilStream,
                                    Action onTimer)
    {
        globalTimeInterval
            .Subscribe(t =>
            {
                timerSubscription?.Dispose();

                timerSubscription =
                    Observable
                    .Interval(TimeSpan.FromSeconds(t))
                    .TakeUntil(takeUntilStream)
                    .Subscribe(_ => onTimer.Invoke())
                    .AddTo(holder);
            })
            .AddTo(holder);
    }

        public void SubscribeToTimer(IDisposable timerSubscription,
                                     Component holder,
                                     Action onTimer)
        {
            globalTimeInterval
                .Subscribe(t =>
                {
                    timerSubscription?.Dispose();

                    timerSubscription =
                        Observable
                        .Interval(TimeSpan.FromSeconds(t))
                        .Subscribe(_ => onTimer.Invoke())
                        .AddTo(holder);
                })
                .AddTo(holder);
        }

}
