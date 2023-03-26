using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    // This is just for testing
    // Will need to grab a reference to the currently selected skill
    // (if not melee) from the Hotbar script
    [SerializeField] ProjectileSpell _projectileSpell;
    [SerializeField] AOESpell _aoeSpell;
    [SerializeField] Transform _projectileSpawnPoint;
    PlayerStateMachine _context;

    void Awake() 
    {
        _context = GetComponent<PlayerStateMachine>();
    }

    public void ProjectileSpell()
    {
        StartCoroutine(ProjectileCoroutine());
    }

    IEnumerator ProjectileCoroutine()
    {
        _context.Animator.SetTrigger(_context.AnimProjectileSpellHash);
        yield return new WaitForSeconds(0.3f);
        switch (_projectileSpell.fireMode) {
            case ProjectileFireMode.Single:
                InitializeProjectile(Instantiate(_projectileSpell.projectilePrefab, _projectileSpawnPoint.position, _context.transform.rotation));
                break;
        }
        _context.Animator.ResetTrigger(_context.AnimProjectileSpellHash);
    }

    private void InitializeProjectile(GameObject projectile) {
        ProjectileController pc = projectile.AddComponent<ProjectileController>() as ProjectileController;
        pc.Damage = _projectileSpell.damage;
        pc.HitEffectPrefab = _projectileSpell.hitEffectPrefab;
        pc.FlashEffectPrefab = _projectileSpell.flashEffectPrefab;
        pc.Speed = _projectileSpell.speed;
        pc.MaxLifetime = 10f;
    }

    public void AOESpell()
    {
        if (_context.CurrentMouseTargetPosition != Vector3.zero)
            StartCoroutine(AOECoroutine());
    }

    IEnumerator AOECoroutine()
    {
        _context.Animator.SetTrigger(_context.AnimAOESpellHash);
        yield return new WaitForSeconds(0.7f);
        GameObject aoeObject = Instantiate(_aoeSpell.AOESpellPrefab, _context.CurrentMouseTargetPosition, Quaternion.identity);
        Destroy(aoeObject, 5f);
    }
}