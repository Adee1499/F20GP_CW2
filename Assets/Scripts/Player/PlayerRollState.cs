using System.Collections;
using UnityEngine;

// TODO: Switch super state to invulnerable during roll
public class PlayerRollState : PlayerBaseState
{
    public PlayerRollState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory) {}

    public override void EnterState() 
    {
        // Just a placeholder for now
        Ctx.StartCoroutine(AnimationTimeout());
    }

    public override void UpdateState() 
    {
        Ctx.AppliedMovement = new Vector3(Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y) * Ctx.WalkSpeed;
    }

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() 
    {
        SwitchState(Factory.Idle());
    }

    IEnumerator AnimationTimeout() 
    {
        Ctx.MovementMultiplier = 2.3f;
        Ctx.Animator.SetTrigger(Ctx.AnimRollHash);
        yield return new WaitForSeconds(1f);
        Ctx.MovementMultiplier = 1f;
        CheckSwitchStates();
    }
}
