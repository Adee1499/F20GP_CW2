using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory) {}

    bool _dealtDamage;

    public override void EnterState() 
    {
        // This is just for testing
        // Once the player is able to swap the skills on the hotbar
        // will need a way of grabbing that skill reference from the hotbar script
        switch (Ctx.CurrentSelectedSkill) {
            case 1:
                // Melee
                _dealtDamage = false;
                WeaponScript.OnWeaponCollidedWithEnemy += DealDamageToEnemy;
                Ctx.StartCoroutine(MeleeAttack());
                break;
            case 2:
                // Projectile spell
                Ctx.StartCoroutine(ProjectileAttack());
                break;
            case 3:
                // AOE spell
                Ctx.StartCoroutine(AOEAttack());
                break;
        }
    }

    public override void UpdateState() 
    {
        Ctx.AppliedMovement = new Vector3(Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y) * Ctx.WalkSpeed;
    }

    public override void ExitState() 
    {
        WeaponScript.OnWeaponCollidedWithEnemy -= DealDamageToEnemy;
    }

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() 
    {
        SwitchState(Factory.Idle());
    }

    IEnumerator MeleeAttack() 
    {
        Ctx.Animator.SetTrigger(Ctx.AnimMeleeAttackHash);
        yield return new WaitForSeconds(1f);
        CheckSwitchStates();
    }

    void DealDamageToEnemy(float damage, EnemyController enemyRef, Vector3 impact)
    {
        if (!_dealtDamage) {
            Debug.Log("Dealing damage to enemy");
            enemyRef.Attacked(damage, impact);
            _dealtDamage = true;
        }
    }

     IEnumerator ProjectileAttack()
    {
        Debug.Log("Casting projectile spell");
        if(Ctx.PlayerMana >= 10) {
            Ctx.UseMana(10);
            Ctx.SpellCaster.ProjectileSpell();
        }
        yield return new WaitForSeconds(0.4f);
        CheckSwitchStates();
    }

    IEnumerator AOEAttack()
    {
        Debug.Log("Casting AOE spell");
        if(Ctx.PlayerMana >= 25 && Ctx.CurrentMouseTargetPosition != Vector3.zero) {
            Ctx.UseMana(25);
            Ctx.SpellCaster.AOESpell();
        }
        yield return new WaitForSeconds(0.4f);
        CheckSwitchStates();
    }
}