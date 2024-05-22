using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Drop", menuName = "SO/Drop")]
public class Drop : ScriptableObject
{
    public Price currency;
    public GameObject prefabDropableGameObject;
}
