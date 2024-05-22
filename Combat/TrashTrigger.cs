using System;
using UnityEngine;

public class TrashTrigger : MonoBehaviour
{
    public event Action onEnterCleaner;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CleanerTrigger cleaner))
        {
            onEnterCleaner?.Invoke();
        }
    }
}
