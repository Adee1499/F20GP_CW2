using System.Collections.Generic;

public class PlayerStateFactory
{
    enum PlayerStates {
        RootState_Default,
        RootState_Invulnerable,
        SubState_Idle,
        SubState_Walk,
        SubState_Run,
        SubState_Attack,
        SubState_Interact
    }

    PlayerStateMachine _context;
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.RootState_Default] = new PlayerDefaultState(_context, this);
        _states[PlayerStates.RootState_Invulnerable] = new PlayerInvulnerableState(_context, this);
        _states[PlayerStates.SubState_Idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.SubState_Walk] = new PlayerWalkState(_context, this);
        _states[PlayerStates.SubState_Run] = new PlayerRunState(_context, this);
        _states[PlayerStates.SubState_Attack] = new PlayerAttackState(_context, this);
        _states[PlayerStates.SubState_Interact] = new PlayerInteractState(_context, this);
    }

    public PlayerBaseState Default()
    {
        return _states[PlayerStates.RootState_Default];
    }

    public PlayerBaseState Invulnerable()
    {
        return _states[PlayerStates.RootState_Invulnerable];
    }

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.SubState_Idle];
    }

    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.SubState_Walk];
    }

    public PlayerBaseState Run()
    {
        return _states[PlayerStates.SubState_Run];
    }

    public PlayerBaseState Attack()
    {
        return _states[PlayerStates.SubState_Attack];
    }

    public PlayerBaseState Interact()
    {
        return _states[PlayerStates.SubState_Interact];
    }
}
