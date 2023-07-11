namespace AfterlifeAdventures.CryRoomStates;

public class CryAvatarRoomState : IState
{
    protected CryAvatarRoomStateMachine stateMachine;

    public CryAvatarRoomState(CryAvatarRoomStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void PhysicsProcess(double delta)
    {

    }

    public virtual void Process(double delta)
    {

    }
}

