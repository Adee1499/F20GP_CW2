using UnityEngine;

public enum SpellType {
    Projectile,
    Beam,
    Lifesteal,
    AreaDamage
}

public abstract class Spell : ScriptableObject
{
    public SpellType spellType;
    public Sprite spellIcon;
    public string spellName;
    [TextArea(3,5)]
    public string description;
    public int damage;
    public int manaCost;
}
