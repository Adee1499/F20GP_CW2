using UnityEngine;

[CreateAssetMenu(menuName = "Spells/AOE Spell")]
public class AOESpell : Spell
{
    public GameObject AOESpellPrefab;

    public void Awake()
    {
        spellType = SpellType.AreaOfEffect;
    }
}
