using UnityEngine;
using UnityEngine.Events;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField] public UnityEvent<Mob> onMobFound;

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Mob mob))
        {
            onMobFound?.Invoke(mob);
        }
    }
}
