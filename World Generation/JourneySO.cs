using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Journey", menuName = "SO/Journey")]
public class JourneySO : ScriptableObject
{
    [SerializeField] public List<Field> queues;

    [System.Serializable]
    public struct Field
    {
        [SerializeField] public MobQueue mobQueue;
        [SerializeField] public UnityEvent onQueueCompleted;

    }
}
