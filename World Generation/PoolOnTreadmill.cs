using UnityEngine;
using Zenject;

public abstract class PoolOnTreadmill<T> : MonoMemoryPool<Vector3, T>
    where T : Component
{
    [Inject] Treadmill treadmill;

    protected override void OnSpawned(T item)
    {
        base.OnSpawned(item);
        item.transform.SetParent(treadmill.transform);
    }
    protected override void Reinitialize(Vector3 position, T item)
    {
        base.Reinitialize(position, item);

        treadmill.AddChild(item.transform, position);
    }
}
