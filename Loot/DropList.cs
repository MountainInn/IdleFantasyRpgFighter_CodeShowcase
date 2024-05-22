using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "DropList", menuName = "SO/DropList")]
public class DropList : ScriptableObject
{
    [SerializeField] public List<Entry> entries = new();

    [Serializable]
    public class Entry
    {
        [Range(0, 1)] public float chance;
        public Drop drop;
    }

    public IEnumerable<Entry> Roll()
    {
        return
            entries
            .Where(e =>
                   (UnityEngine.Random.value < e.chance));
    }
}
