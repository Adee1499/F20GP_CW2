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
                // Spell projectile
                Ctx.StartCoroutine(ProjectileAttack());
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

    void DealDamageToEnemy()
    {
        if (!_dealtDamage) {
            Debug.Log("Dealing damage to enemy");
            _dealtDamage = true;
        }
    }

    IEnumerator ProjectileAttack()
    {
        Debug.Log("Casting spell");
        Ctx.SpellCaster.SpawnProjectile();
        yield return new WaitForSeconds(0.4f);
        CheckSwitchStates();
    }
}