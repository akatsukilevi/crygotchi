namespace AfterlifeAdventures.CryRoomStates;

public class CryRoomChangedPanicState : CryOnGroundState
{
    private readonly RoomState _room;

    public CryRoomChangedPanicState(CryAvatarRoomStateMachine stateMachine, RoomState roomState) : base(stateMachine)
    {
        this._room = roomState;
    }

    public override void Enter()
    {
        base.Enter();

        //* Is the current tile valid?
        var tile = this._room.GetTileAt(this.stateMachine.Avatar.GetCurrentPosition());

        if (tile == null || tile.Decoration != null)
        {
            //* If not, teleport to a random valid tile
            this.stateMachine.Avatar.GlobalPosition = this.stateMachine.Avatar.TeleportToRandomTile();
        }

        //* Then, return back to the idle state
        this.stateMachine.ChangeState(this.stateMachine.IdleState);
    }
}
