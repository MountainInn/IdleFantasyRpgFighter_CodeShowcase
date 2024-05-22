using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class RoadCaster : MonoBehaviour
{
    [SerializeField] GeneralLayerMask landMask;
    [SerializeField] float verticalPadding = .1f;
    [SerializeField] float raycastLength = 1000;

    [Inject] GameSettings gameSettings;
    [Inject] Treadmill treadmill;

    RaycastHit[] hits = new RaycastHit[1];

    public float3 CastDown(Vector3 transformPosition, float3 pos)
    {
        return CastDown(transformPosition, pos, verticalPadding);
    }

    public float3 CastDown(Vector3 transformPosition, float3 pos, float verticalPadding)
    {
        var raycastStart = pos;

        raycastStart.x += transformPosition.x;
        raycastStart.x -= treadmill.Position.x;

        raycastStart.y = gameSettings.roadSpawnHeight;

        raycastStart.z += transformPosition.z;
        raycastStart.z -= treadmill.Position.z;

        pos = Cast(pos, verticalPadding, raycastStart);

        return pos;
    }

    public float3 CastDownWithoutTreadmil(float3 pos, float verticalPadding)
    {
        var raycastStart = pos;

        raycastStart.y = gameSettings.roadSpawnHeight;

        pos = Cast(pos, verticalPadding, raycastStart);

        return pos;
    }

    float3 Cast(float3 pos, float verticalPadding, float3 raycastStart)
    {
        if (Physics.RaycastNonAlloc(raycastStart,
                                    Vector3.down,
                                    hits,
                                    raycastLength,
                                    landMask.layers.value,
                                    QueryTriggerInteraction.Collide) > 0)
        {
            pos.y = hits[0].point.y + verticalPadding;

            // Debug.DrawRay(raycastStart, Vector3.down * hits[0].distance, Color.cyan, 20f);
        }
        else
        {
            // Debug.DrawRay(raycastStart, Vector3.down * raycastLength, Color.red, 20f);
        }

        return pos;
    }
}
