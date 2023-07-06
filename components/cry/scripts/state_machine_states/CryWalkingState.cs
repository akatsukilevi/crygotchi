namespace Crygotchi.CryRoomStates;
public class CryWalkingState : CryOnGroundState
{
    public CryWalkingState(CryAvatarRoomStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Avatar.AAHelper.AnimParams["Walking"] = true;
        stateMachine.Avatar.AAHelper.AnimParams["Sprinting"] = false;
    }

    public override void Process(double delta)
    {
        base.Process(delta);

        //Time to switch back to idle
        if(stateMachine.Avatar.Position.DistanceTo(targetPosition) < 0.1f)
        {
            //snap to position
            stateMachine.Avatar.GlobalPosition = targetPosition;
            //Change to Idle
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}

