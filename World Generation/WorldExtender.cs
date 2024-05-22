using Unity.Mathematics;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Splines;
using Zenject;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class WorldExtender : MonoBehaviour
{
    [SerializeField] public UnityEvent<Road> onRoadSpawned = new();

    [Inject] GameSettings gameSettings;
    [Inject] Treadmill treadmill;
    [Inject] Land.Pool landPool;
    [Inject] Road.Pool roadPool;

    Dictionary<Vector2Int, Land> chunks = new();
    public Queue<Road> roads {get; private set;} = new();
    bool isReadyForRoads;

    public Vector2Int TreadmillChunkIndices
    {
        get => GetChunkIndices(treadmill.Position);
    }
   
    public Vector2Int GetChunkIndices(Vector3 position) =>
        (position / gameSettings.landSize.Value) .xz() .FloorToVector2Int();


    public Vector3 GetChunkStartPosition(Vector2Int chunkIndices) =>
        new Vector3(chunkIndices.x, 0, chunkIndices.y) * gameSettings.landSize.Value;

    public IObservable<Vector2Int> ObserveTreadmillChunkIndices()
    {
        return
            Observable
            .Interval(TimeSpan.FromSeconds(0.25f))
            .Select(_ => TreadmillChunkIndices)
            .Distinct();
    }

    void Awake()
    {
        ObserveTreadmillChunkIndices()
            .Subscribe(indices =>
            {
                Vector2Int fromIndices = indices - new Vector2Int(gameSettings.worldSize, gameSettings.worldSize);
                Vector2Int toIndices = indices + new Vector2Int(gameSettings.worldSize, gameSettings.worldSize);;
                SpawnLands(fromIndices, toIndices);
            })
            .AddTo(this);
    }

    void Start()
    {
        ExtendRoad().Forget();
    }

    void SpawnLands(Vector2Int fromIndices, Vector2Int toIndices)
    {
        isReadyForRoads = false;
       
        List<IObservable<bool>> observeLandGenerated = new();

        foreach (var (indices, land) in chunks.ToArray())
        {
            if (!(fromIndices.x <= indices.x && indices.x <= toIndices.x &&
                  fromIndices.y <= indices.y && indices.y <= toIndices.y))
            {
                chunks.Remove(indices, out Land oldLand);

                landPool.Despawn(oldLand);
            }
        }

        for (int y = fromIndices.y; y <= toIndices.y; y++)
        {
            for (int x = fromIndices.x; x <= toIndices.x; x++)
            {
                Vector2Int indices = new Vector2Int(x, y);

                if (chunks.TryGetValue(indices, out Land land))
                    continue;

                Vector3 spawnPosition = GetChunkStartPosition(indices);

                land = landPool.Spawn(spawnPosition);

                land.gameObject.name = $"Land {indices}";

                chunks.Add(indices, land);

                land.RebuildOnStart(spawnPosition);

                observeLandGenerated.Add(land.generated);
            }
        }

        observeLandGenerated
            .CombineLatestValuesAreAllTrue()
            .Take(1)
            .Subscribe(_ => isReadyForRoads = true);
    }

    public async UniTask ExtendRoadFromLast()
    {
        await UniTask
            .WaitUntil(() => isReadyForRoads)
            .ContinueWith(() => UniTask.NextFrame());
       
        Road lastRoad = roads.Dequeue();

        BezierKnot lastKnot = lastRoad.singleSpline.Knots.Last();

        ExtendRoad(lastKnot);

        roadPool.Despawn(lastRoad);
    }

    async UniTask ExtendRoad()
    {
        await UniTask
            .WaitUntil(() => isReadyForRoads)
            .ContinueWith(() => UniTask.NextFrame());

        BezierKnot defaultKnot = new BezierKnot(float3.zero, float3.zero, float3.zero, quaternion.identity);

        ExtendRoad(defaultKnot);
    }

    void ExtendRoad(BezierKnot lastKnot)
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0);

        Road newRoad = roadPool.Spawn(spawnPosition);

        Spline newSpline = newRoad.singleSpline;

        newSpline.Clear();

        newSpline.Add(lastKnot);

        float randomAngle =
            UnityEngine.Random.Range(gameSettings.roadMinimalTurnAngle,
                                     gameSettings.roadMaximumTurnAngle)
            *
            (UnityEngine.Random.value < .5f ? -1 : 1);

        Quaternion knotRotation = lastKnot.Rotation * Quaternion.Euler(0, randomAngle, 0);

        Vector3 knotPosition =
            (Vector3)lastKnot.Position
            +
            (Vector3) Unity.Mathematics.math.rotate(knotRotation, new float3(0, 0, 1))
            * gameSettings.roadSegmentLength;

        BezierKnot nextKnot =
            new BezierKnot(knotPosition,
                           new float3(0, 0, -gameSettings.roadTangentLength),
                           new float3(0, 0, gameSettings.roadTangentLength),
                           knotRotation);

        newSpline.Add(nextKnot);

        newSpline.SetTangentMode(TangentMode.Mirrored);

        roads.Enqueue(newRoad);

        var roadStartChunkIndices = GetChunkIndices(lastKnot.Position);
        var roadEndChunkIndices = GetChunkIndices(knotPosition);

        if (!chunks.TryGetValue(roadEndChunkIndices, out Land value))
        {
            SpawnLands(roadStartChunkIndices,
                       roadEndChunkIndices);
        }

        onRoadSpawned?.Invoke(newRoad);
    }
}
