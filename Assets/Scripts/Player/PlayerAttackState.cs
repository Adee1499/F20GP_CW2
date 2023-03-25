using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory) {}

    bool _dealtDamage;

    public override void EnterState() 
    {
        _dealtDamage = false;
        WeaponScript.OnWeaponCollidedWithEnemy += DealDamageToEnemy;
        Ctx.StartCoroutine(AnimationTimeout());
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

    IEnumerator AnimationTimeout() 
    {
        Ctx.Animator.SetTrigger(Ctx.AnimAttackHash);
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
}
