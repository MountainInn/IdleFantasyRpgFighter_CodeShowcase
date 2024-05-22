using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    public Vector3 Position
    {
        get => _position;
        set
        {
            _position.x = value.x;
            _position.y = 0;
            _position.z = value.z;

            foreach (var child in absolutePosition.Keys)
            {
                UpdateChildPosition(child);
            }
        }
    }
    public Vector3 _position;

    Dictionary<Transform, Vector3> absolutePosition = new();

    void Start()
    {
        Position = Vector3.zero;
    }

    public Vector3 ToAbsolutePosition(Vector3 relativePosition)
    {
        return relativePosition + Position;
    }

    public Vector3 ToRelativePosition(Vector3 absolutePosition)
    {
        return absolutePosition - Position;
    }

    public Vector3 GetAbsolutePosition(Transform child)
    {
        return absolutePosition[child];
    }

    public void AddChild(Transform child, Vector3 childAbsolutePosition)
    {
        if (absolutePosition.ContainsKey(child))
        {
            absolutePosition[child] = childAbsolutePosition;
        }
        else
        {
            absolutePosition.Add(child, childAbsolutePosition);
        }

        UpdateChildPosition(child);

        child.SetParent(transform);
    }

    void UpdateChildPosition(Transform child)
    {
        child.position = ToRelativePosition(absolutePosition[child]);
    }
}
