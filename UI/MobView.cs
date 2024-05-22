using UnityEngine;

public class MobView : MonoBehaviour
{
    [SerializeField] protected ProgressBar healthBar;
    [SerializeField] protected ProgressBar attackTimerBar;
    [SerializeField] protected ProgressBar energyBar;


    public void Subscribe(Mob mob)
    {
        healthBar.Subscribe(mob.gameObject, mob.health);
        attackTimerBar.Subscribe(mob.gameObject, mob.attackTimer);
    }
}
