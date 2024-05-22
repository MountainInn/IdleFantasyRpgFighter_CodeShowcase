using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class SplineMove : MySplineComponent
{
    [SerializeField][Range(1, 10f)] float verticalPadding = 2f;
    [Space]
    [SerializeField][Range(0, 1f)] float characterStepHeight = 0.5f;
    [SerializeField][Range(1, 10f)] float characterStepFrequency = 1;
    [SerializeField] Transform character;

    [HideInInspector] public UnityEvent onRoadEnd;

    [Inject] GameSettings gameSettings;
    [Inject] Treadmill treadmill;
    [Inject] WorldExtender worldExtender;
    [Inject] RoadCaster roadCaster;
    [Inject]
    public void Construct()
    {
        onRoadEnd.AddListener(() => worldExtender.ExtendRoadFromLast().Forget());

        worldExtender.onRoadSpawned.AddListener(SwitchToNextRoad);
    }

    public void SwitchToNextRoad(Road road)
    {
        if (road != null && road.Container != null)
        {
            if (targetSpline.Value == null)
            {
                targetSpline.Value = road.Container;
                path = 0f;
                ratio = 0f;
            }
        }
    }

    public bool isMoving = true;

    float speed => gameSettings.movementSpeed;

    RaycastHit[] hits = new RaycastHit[1];

    public void UpdateTransform(Transform transform, float ratio)
    {
        EvaluatePositionAndRotation(ratio, out var splinePosition, out var rotation);

        transform.rotation = rotation;

        float splineY = splinePosition.y;

        splinePosition += treadmill.Position;

        treadmill.Position = splinePosition;

        splinePosition.y = splineY;

        var rayStart = splinePosition - treadmill.Position;

        Vector3 castPosition = roadCaster.CastDownWithoutTreadmil(rayStart,
                                                                  verticalPadding);

        float characterY =
            castPosition.y
            + SineStep(ratio);

        transform.position = transform.position.WithY(characterY);
    }

    private float SineStep(float ratio)
    {
        return
            Mathf.Sin(path * characterStepFrequency)
            * characterStepHeight;
    }

    float
        path = 0f,
        ratio = 0f;

    void Update()
    {
        if (!isMoving)
            return;

        if (targetSpline.Value == null)
            return;

        if (ratio >= 1f)
        {
            onRoadEnd?.Invoke();
            targetSpline.Value = null;
            return;
        }

        path += speed * Time.deltaTime;

        ratio = path / targetSpline.Value.CalculateLength();

        UpdateTransform(character, ratio);
    }
}
