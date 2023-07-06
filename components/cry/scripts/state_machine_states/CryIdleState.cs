namespace Crygotchi.CryRoomStates;
public class CryIdleState : CryOnGroundState
{
    private double _timer = 0.0f;
    public CryIdleState(CryAvatarRoomStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Avatar.AAHelper.AnimParams["Walking"] = false;
        stateMachine.Avatar.AAHelper.AnimParams["Sprinting"] = false;

        _timer = 5.0;
    }
    public override void Process(double delta)
    {
        base.Process(delta);
        //Time to switch to walking
        if (_timer <= 0.0)
        {
            //Find new target
            targetPosition = stateMachine.Avatar.GetNextTargetTilePosition();
            //Change to Walking
            stateMachine.ChangeState(stateMachine.WalkingState);
        }
        //timers!
        _timer -= delta;
    }

}

