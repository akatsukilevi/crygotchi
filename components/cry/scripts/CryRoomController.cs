using System.Collections.Generic;

namespace AfterlifeAdventures;

public partial class CryRoomController : CharacterBody3D
{
    [ExportCategory("Settings")]
    [ExportGroup("References")]
    [Export] public AvatarAnimationHelper AAHelper;
    [ExportGroup("Timers")]
    [Export] public float MoveTime = 10f;

    private RoomState _room;

    private Vector2 _currentTile = Vector2.Zero;
    private Vector2 _previousTile = Vector2.Zero;
    private bool _isExploringMode = false;
    private bool _isCryVisible = false;

    private float _currentTime = 0f;

    private CryAvatarRoomStateMachine _controllerStateMachine;

    public override void _Ready()
    {
        base._Ready();

        this._room = this.GetNode<RoomState>("/root/RoomState");
        this._room.OnStateChange += this.OnRoomUpdate;

        this._controllerStateMachine = new CryAvatarRoomStateMachine(this, this._room);
        this._controllerStateMachine.ChangeState(_controllerStateMachine.IdleState);

        this.GetNextTargetTilePosition();
        this.GlobalPosition += Vector3.Up;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!this._isExploringMode || !this._isCryVisible)
        {
            Visible = false;
            AAHelper.Animator.Active = false;
            return;
        }

        Visible = true;
        AAHelper.Animator.Active = true;
        _controllerStateMachine.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _controllerStateMachine.PhysicsProcess(delta);
    }

    private void OnRoomUpdate(bool _)
    {
        //* Change the current state to the room change state
        this._controllerStateMachine.ChangeState(this._controllerStateMachine.RoomChangedPanicState);

        //* Check if it is in exporing mode
        this._isExploringMode = this._room.GetMode() == RoomMode.Exploring;

        this.GetNextTargetTilePosition();
    }

    public Vector2 GetCurrentPosition()
    {
        return this._currentTile;
    }

    public Vector3 TeleportToRandomTile()
    {
        var target = this._room.GetRandomTile();

        //* Has valid position here, set as current and previous, and just continue
        this._previousTile = target?.Key ?? this._previousTile;
        this._currentTile = target?.Key ?? this._currentTile;
        this._isCryVisible = target != null;

        return new Vector3(this._currentTile.X * 2, 1f, this._currentTile.Y * 2);
    }

    //TODO: Add GetNextTilePosition method that will be used by state machine.
    //TODO: Parametrize times between states
    public Vector3 GetNextTargetTilePosition()
    {
        if (!this._isExploringMode) return GlobalPosition;

        //* Grab the tile in all 8 directions around the current one
        List<RoomTileInstance> tiles = new()
        {
            this._room.GetTileAt(this._currentTile + new Vector2(-1, 1)), // top_left
            this._room.GetTileAt(this._currentTile + new Vector2(0, 1)), // top_center
            this._room.GetTileAt(this._currentTile + new Vector2(1, 1)), // top_right
            this._room.GetTileAt(this._currentTile + new Vector2(-1, 0)), // middle_left
            this._room.GetTileAt(this._currentTile + new Vector2(0, 0)), // middle_center
            this._room.GetTileAt(this._currentTile + new Vector2(1, 0)), // middle_right
            this._room.GetTileAt(this._currentTile + new Vector2(-1, -1)), // bottom_left
            this._room.GetTileAt(this._currentTile + new Vector2(0, -1)), // bottom_center
            this._room.GetTileAt(this._currentTile + new Vector2(1, -1)), // bottom_right
        };
        //* Ignore all tiles that are not valid
        tiles.RemoveAll(x => x == null);

        //* Ignore the previous and current tile
        tiles.RemoveAll(x => x.Position == this._previousTile);
        tiles.RemoveAll(x => x.Position == this._currentTile);

        //* If no tiles left, means we're stuck, teleport somewhere that is not stuck, go there
        if (tiles.Count == 0)
        {
            this.GlobalPosition = this.TeleportToRandomTile();

            if (this._isCryVisible) GD.Print($"[ CRY ] Teleporting to [{this._currentTile.X}, {this._currentTile.Y}]");
            return GlobalPosition;
        }

        //* There are tiles left, pick a random and set as current one
        this._previousTile = this._currentTile;
        this._currentTile = tiles.ElementAt(Random.Shared.Next(0, tiles.Count - 1)).Position;
        this._isCryVisible = true;

        GD.Print($"[ CRY ] Moving to [{this._currentTile.X}, {this._currentTile.Y}] from [{this._previousTile.X}, {this._previousTile.Y}]");
        return new Vector3(this._currentTile.X * 2, 1f, this._currentTile.Y * 2);
    }
}
