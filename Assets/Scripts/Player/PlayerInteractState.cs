using System.Collections;
using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    public PlayerInteractState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory) {}

    public override void EnterState() 
    {
        // Just a placeholder for now
        Ctx.StartCoroutine(AnimationTimeout());
    }

    public override void UpdateState() {}

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() 
    {
        SwitchState(Factory.Idle());
    }

    IEnumerator AnimationTimeout() 
    {
        Ctx.Animator.SetTrigger(Ctx.AnimPickUpHash);
        Debug.Log("Pick up");
        yield return new WaitForSeconds(0.5f);
        CheckSwitchStates();
    }
}
