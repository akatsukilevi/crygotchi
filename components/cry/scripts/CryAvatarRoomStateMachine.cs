namespace Crygotchi;

public class CryAvatarRoomStateMachine : StateMachine
{
    public CryRoomController Avatar;

    public CryRoomStates.CryIdleState IdleState { get; }
    public CryRoomStates.CryWalkingState WalkingState { get; }
    public CryRoomStates.CryRoomChangedPanicState RoomChangedPanicState { get; }

    public CryAvatarRoomStateMachine(CryRoomController avatarController, RoomState roomState)
    {
        this.Avatar = avatarController;

        this.IdleState = new(this);
        this.WalkingState = new(this);
        this.RoomChangedPanicState = new(this, roomState);
    }

}
