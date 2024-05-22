using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PropSO", menuName = "SO/Props/PropSO")]
public class PropSO : ScriptableObject
{
    [SerializeField] public Sprite sprite;
    [Header("Restoration")]
    [SerializeField] public float healingAmount;
    [SerializeField] public float staminaAmount;
    [SerializeField] public float manaAmount;
    [Space]
    [SerializeField] public DropList dropList;

}
