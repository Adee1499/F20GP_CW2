using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies")]
public class EnemyStats : ScriptableObject
{
    public new string name;

    [Header("Health and Mana")]
    // maximum and current hit points
    public float maxHealth;
    // maximum and current mana points for spellcaster enemies
    public float maxMana;
    // armour, could either dampen damage or act as overhealth
    public float armour;

    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;

    [Header("Vision and Engangement Ranges")]
    public float detectionRange;
    public float combatRange;
}
