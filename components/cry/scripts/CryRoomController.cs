using System.Collections.Generic;

namespace Crygotchi;

public partial class CryRoomController : RigidBody3D
{
    [ExportCategory("Settings")]
    [ExportGroup("References")]
    [Export] public AnimationTree CryAnimationTree;
    [ExportGroup("Timers")]
    [Export] public float MoveTime = 10f;

    private RoomState _room;

    private CryAvatarRoomStateMachine _crStateMachine;

    private Vector2 _currentTile = Vector2.Zero;
    private Vector2 _previousTile = Vector2.Zero;
    private bool _isExploringMode = false;
    private bool _isCryVisible = false;

    private float _currentTime = 0f;

    public override void _Ready()
    {
        base._Ready();
        _crStateMachine = new CryAvatarRoomStateMachine();
        this._room = this.GetNode<RoomState>("/root/RoomState");

        this._room.OnStateChange += this.OnRoomUpdate;
        this.UpdatePosition();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        //Switch to state machine


        if (!this._isExploringMode || !this._isCryVisible)
        {
            //* Hide cry then exit
            Visible = false;
            _currentTime = 0f;
            return;
        }

        //* Make cry visible and move to the right position
        Visible = true;
        GlobalPosition = new Vector3(this._currentTile.X * 2, 1f, this._currentTile.Y * 2);

        //* Update the timer
        _currentTime += 0.1f;
        if (_currentTime >= this.MoveTime)
        {
            _currentTime = 0f;
            UpdatePosition();
        }
    }

    private void OnRoomUpdate(bool _)
    {
        //* Check if it is in exporing mode
        this._isExploringMode = this._room.GetMode() == RoomMode.Exploring;

        //* Check if current tile is still valid floor, move if not
        this.UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!this._isExploringMode) return;

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

        //* Ignore the previous tile
        tiles.RemoveAll(x => x.Position == this._previousTile);

        //* If no tiles left, means we're stuck, teleport somewhere that is not stuck, go there
        if (tiles.Count == 0)
        {
            var target = this._room.GetRandomTile();

            //* Has valid position here, set as current and previous, and just continue
            this._previousTile = target?.Key ?? this._previousTile;
            this._currentTile = target?.Key ?? this._currentTile;
            this._isCryVisible = target != null;

            if (this._isCryVisible) GD.Print($"[ CRY ] Teleporting to [{this._currentTile.X}, {this._currentTile.Y}]");
            return;
        }

        //* There are tiles left, pick a random and set as current one
        this._previousTile = this._currentTile;
        this._currentTile = tiles.ElementAt(Random.Shared.Next(0, tiles.Count - 1)).Position;
        this._isCryVisible = true;
        GD.Print($"[ CRY ] Moving to [{this._currentTile.X}, {this._currentTile.Y}] from [{this._previousTile.X}, {this._previousTile.Y}]");
    }
}
