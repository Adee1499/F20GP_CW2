using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;

    [Header("Health and Mana")]
    // maximum and current hit points
    [SerializeField] float _maxHealth;
    [SerializeField] float _health;
    // maximum and current mana points for spellcaster enemies
    [SerializeField] float _maxMana;
    [SerializeField] float _mana;
    // armour, could either dampen damage or act as overhealth
    [SerializeField] float _armour;

    [Header("Movement")]
    // different movement speeds
    [SerializeField] float _walkSpeed;
    [SerializeField] float _runSpeed;

    [Header("Vision and Engangement Ranges")]
    // these ranges dictate behaviour changes
    [SerializeField] float _detectionRange;
    [SerializeField] float _combatRange;

    // getters setters
    public float MaxHealth { get {return _maxHealth;} set { _maxHealth = value;}}
    public float CurrentHealth { get {return _health;} set { _health = value;}}
    public float MaxMana { get {return _maxMana;} set { _maxMana = value;}}
    public float CurrentMana { get {return _mana;} set { _mana = value;}}
    public float Armour { get {return _armour;} set { _armour = value;}}
    public float WalkSpeed { get {return _walkSpeed;} set { _walkSpeed = value;}}
    public float RunSpeed { get {return _runSpeed;} set { _runSpeed = value;}}
    public float DetectionRange { get {return _detectionRange;} set { _detectionRange = value;}}
    public float CombatRange { get {return _combatRange;} set { _combatRange = value;}}

    private void Awake() {
        // set health
        _maxHealth = stats.maxHealth;
        _health = _maxHealth;
        // set mana
        _maxMana = stats.maxMana;
        _mana = _maxMana;
        // set armour
        _armour = stats.armour;
        // set movement speed
        _walkSpeed = stats.walkSpeed;
        _runSpeed = stats.runSpeed;
        // set vision limits
        _detectionRange = stats.detectionRange;
        _combatRange = stats.combatRange;
    }

    // change the health by a value and return result
    public float ModifyHealth(float value) {
        _health += value;
        return _health; 
    }

    // change the mana by a value and return result
    public float ModifyMana(float value) {
        _mana += value;
        return _mana;
    }
}
