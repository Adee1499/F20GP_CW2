using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory) {}

    public override void EnterState() 
    {
        Debug.Log("Entered attack state");
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() {}
}
