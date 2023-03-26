using UnityEngine;

public enum ProjectileFireMode {
    Single,
    Triple,
    Radial
}

[CreateAssetMenu(menuName = "Spells/Projectile Spell")]
public class ProjectileSpell : Spell
{
    public ProjectileFireMode fireMode;
    public GameObject projectilePrefab;
    public GameObject hitEffectPrefab;
    public GameObject flashEffectPrefab;    
    public float speed;

    public void Awake()
    {
        spellType = SpellType.Projectile;
    }
}
