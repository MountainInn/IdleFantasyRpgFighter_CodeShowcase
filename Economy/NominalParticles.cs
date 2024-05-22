using System.Collections.Generic;
using UnityEngine;

public class NominalParticles : MonoBehaviour
{
    [SerializeField] List<Field> fields;

    public List<Field> Fields => fields;

    void OnEnable()
    {
        fields.Sort((a, b) => b.amount.CompareTo(a.amount));
    }

    [System.Serializable]
    public class Field
    {
        public int amount;
        public LootParticles particles;
    }
}
