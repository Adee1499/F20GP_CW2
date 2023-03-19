public class PlayerDefaultState : PlayerBaseState
{
    public PlayerDefaultState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) 
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void UpdateState()
    {
        // TODO: Move take damage method to PlayerDefaultState
        // Check for enemy hit
        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void InitializeSubState() 
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed) {
            SetSubState(Factory.Idle());
        } else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed) {
            SetSubState(Factory.Walk());
        } else if (Ctx.IsMovementPressed && Ctx.IsRunPressed) {
            SetSubState(Factory.Run());
        }
    }

    public override void CheckSwitchStates() {}
}
