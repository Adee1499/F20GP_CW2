using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory) {}

    public override void EnterState() 
    {
        Ctx.Animator.SetBool(Ctx.AnimIsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.AnimIsRunningHash, true);
    }

    public override void UpdateState() 
    {
        Ctx.AppliedMovement = new Vector3(Ctx.CurrentMovementInput.x, 0f, Ctx.CurrentMovementInput.y) * Ctx.RunSpeed;
        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() {
        if (!Ctx.IsMovementPressed) {
            SwitchState(Factory.Idle());
        } else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed) {
            SwitchState(Factory.Walk());
        }
    }
}