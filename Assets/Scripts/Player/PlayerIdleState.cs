using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory) {}

    public override void EnterState()
    {
        Ctx.AppliedMovement = Vector3.zero;
    }

    public override void UpdateState()
    {
        Debug.Log("Idle state");
        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() {
        if (Ctx.IsMovementPressed && Ctx.IsRunPressed) {
            SwitchState(Factory.Run());
        } else if (Ctx.IsMovementPressed) {
            SwitchState(Factory.Walk());
        } else if (Ctx.IsAttackPressed) {
            SwitchState(Factory.Attack());    
        } else if (Ctx.IsInteractPressed) {
            SwitchState(Factory.Interact());
        }
    }
}
