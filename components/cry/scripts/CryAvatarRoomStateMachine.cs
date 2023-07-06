namespace Crygotchi;
public class CryAvatarRoomStateMachine : StateMachine
{
    //public RoomState _roomState;
    public CryRoomController Avatar;

    public CryRoomStates.CryIdleState IdleState { get; }
    public CryRoomStates.CryWalkingState WalkingState { get; }
    //public PlayerJogState JogState { get; }
    //public PlayerSprintState SprintState { get; }

    public CryAvatarRoomStateMachine(CryRoomController avatarController)
    {
        Avatar = avatarController;

        IdleState = new CryRoomStates.CryIdleState(this);
        WalkingState = new CryRoomStates.CryWalkingState(this);
        //JogState = new PlayerJogState(this);
        //SprintState = new PlayerSprintState(this);
    }

}
