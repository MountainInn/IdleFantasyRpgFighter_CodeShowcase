using UnityEngine;

[CreateAssetMenu(fileName = "MobStats", menuName = "SO/Stats/Mob Stats")]
public class MobStatsSO : ScriptableObject
{
    [SerializeField] public Sprite sprite;
    [Space]
    [SerializeField] public DropList dropList;
    [Space]
    [SerializeField] public Sprite icon;
    [Header("Stats")]
    [SerializeField] public Stat health;
    [SerializeField] public Stat mana;
    [SerializeField] public Stat stamina;
    [Space]
    [SerializeField] public Stat strength;
    [SerializeField] public Stat intelligence;
    [Space]
    [SerializeField] public Stat agility;
    [SerializeField] public Stat perception;
    [SerializeField] public Stat luck;
    [Space]
    [SerializeField] public Stat speed;
    [SerializeField] public Stat magicSpeed;
    [Space]
    [SerializeField] public Stat critChance_Phys;
    [SerializeField] public Stat critChance_Mag;
    [SerializeField] public Stat critMult;
    [Space]
    [SerializeField] public Stat attackTimer;

    public StatsSO ToStats()
    {
        var stats = new StatsSO
        {
            health = this.health,
            mana = this.mana,
            stamina = this.stamina,
            strength = this.strength,
            intelligence = this.intelligence,
            perception = this.perception,
            luck = this.luck,
            agility = this.agility,
            speed = this.speed,
            magicSpeed = this.magicSpeed,
            critChance_Phys = this.critChance_Phys,
            critChance_Mag = this.critChance_Mag,
            critMult = this.critMult,
            attackTimer = this.attackTimer,
        };

        stats.CalculateAll();

        return stats;
    }

    public void Apply(Mob mob)
    {
        this . ToStats() . Apply(mob);

        mob.spriteRenderer.sprite = sprite;
        mob.dropList = this.dropList;
    }

    public void Apply(Ally ally)
    {
        this . ToStats() . Apply(ally);
    }

    public void Multiply(float multiplier)
    {
        // health *= multiplier;
        // attackDamage *= multiplier;
    }
}
