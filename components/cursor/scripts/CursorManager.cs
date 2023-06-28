namespace Crygotchi;

public partial class CursorManager : Node
{
    [ExportCategory("General")]
    [ExportGroup("Input")]
    [Export] private float Speed = 25f;
    [ExportGroup("References")]
    [Export] private Node3D IconBase;
    [Export] private Node3D CursorBase;

    [ExportCategory("Apppearance")]
    [ExportGroup("Icons")]
    [Export] private Node3D IconRemove;
    [Export] private RoomTileObject IconTile;
    [Export] private Node3D IconItem;

    [ExportGroup("Colors")]
    [Export] private Color NormalColor;
    [Export] private Color HighlightColor;
    [Export] private Color PositiveColor;
    [Export] private Color NegativeColor;

    private CursorState _cursorState;
    private RoomState _roomState;

    private MeshInstance3D _cursorIndicator;
    private MeshInstance3D _cursorBorder;
    private StandardMaterial3D _cursorMat;
    private Color _targetColor;

    private AnimationPlayer _animator;
    private Node3D _holdingIcon;
    private Node3D _parent;

    public override void _Ready()
    {
        base._Ready();

        this._cursorState = GetNode<CursorState>("/root/CursorState");
        this._roomState = GetNode<RoomState>("/root/RoomState");

        this._animator = GetNode<AnimationPlayer>("../Mesh/AnimationPlayer");
        this._parent = GetNode<Node3D>("..");

        this._cursorIndicator = this.CursorBase.GetNode<MeshInstance3D>("./Cursor Walls");
        this._cursorBorder = this.CursorBase.GetNode<MeshInstance3D>("./Cursor Indicator");

        this._cursorMat = new StandardMaterial3D();
        this._cursorIndicator.MaterialOverride = this._cursorMat;
        this._cursorBorder.MaterialOverride = this._cursorMat;

        this._animator.Play("Cursor Idle");

        this._cursorState.OnStateChange += OnStateUpdate;
        this._cursorState.OnItemChange += OnItemUpdate;
        this._roomState.OnStateChange += (bool _) => OnStateUpdate();

        this.OnStateUpdate();
    }

    private void OnStateUpdate()
    {
        //* Should check what Icon will be showing
        var mode = this._roomState.GetMode();

        switch (mode)
        {
            case RoomMode.Exploring:
                this.StateUpdateExploring();
                break;
            case RoomMode.Building:
                this.StateUpdateBuilding();
                break;
            case RoomMode.Decorating:
                this.StateUpdateDecorating();
                break;
        }
    }

    private void OnItemUpdate()
    {
        //* Ensure to clean the current item icon
        if (this._holdingIcon != null)
        {
            this._holdingIcon.QueueFree();
            this._holdingIcon = null;
        }

        //* Is holding a item?
        if (this._cursorState.IsHoldingItem())
        {
            //* Set it as visible then
            var item = this._cursorState.PeekItem();
            this._holdingIcon = item.IconMesh.Instantiate<Node3D>();
            this.IconItem.AddChild(this._holdingIcon);
        }
    }

    private void StateUpdateExploring()
    {
        var position = this._cursorState.GetPosition();
        var currentHovering = this._roomState.GetTileAt(position);
        var currentDecoration = currentHovering?.Decoration;

        //* Change the visible icon
        this.IconRemove.Visible = false;
        this.IconTile.Visible = false;
        this.IconItem.Visible = true;

        //* Check if hovering some item
        if (currentHovering == null || currentDecoration == null)
        {
            this._targetColor = NormalColor;
            return;
        }

        var decoEntry = currentDecoration?.DecorationEntry;
        this._targetColor = !decoEntry.IsInteractable ? NormalColor : HighlightColor;
        return;
    }

    private void StateUpdateBuilding()
    {
        var position = this._cursorState.GetPosition();
        var currentHovering = this._roomState.GetTileAt(position);
        var currentSelected = this._roomState.GetSelectedBuilding();

        //* Hide the holding item icon
        this.IconItem.Visible = false;

        //* Update the texture color of the icon tile
        if (currentSelected != null) this.IconTile.SetupPreview(currentSelected);

        //* On building mode, is focusing a tile?
        if (currentHovering == null)
        {
            if (currentSelected == null)
            {
                //* Not focusing anything, but nothing selected
                this._targetColor = NormalColor;
                this.IconRemove.Visible = false;
                this.IconTile.Visible = false;
                return;
            }

            //* Nope, not focusing anything
            this._targetColor = PositiveColor;
            this.IconRemove.Visible = false;
            this.IconTile.Visible = true;
            return;
        }

        //* Yep, focusing something
        this._targetColor = NegativeColor;
        this.IconRemove.Visible = true;
        this.IconTile.Visible = false;
    }

    private void StateUpdateDecorating()
    {
        var position = this._cursorState.GetPosition();
        var currentHovering = this._roomState.GetTileAt(position);
        this.IconTile.Visible = false;
        this.IconRemove.Visible = false;

        //* Hide the holding item icon
        this.IconItem.Visible = false;

        //* Not focusing anything, can't do anything
        if (currentHovering == null)
        {
            this._targetColor = NormalColor;
            return;
        }

        if (currentHovering.Decoration == null)
        {
            //* Has no decoration, can add one
            this._targetColor = PositiveColor;
            return;
        }

        //* Has decoration, can remove it
        this._targetColor = NegativeColor;
        this.IconRemove.Visible = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        var pos = this._cursorState.GetPosition();

        //* Interpolate current position
        this._parent.Position = this._parent.Position.Lerp(
            new Vector3(pos.X * 2, 0f, pos.Y * 2),
            (float)delta * this.Speed
        );

        //* Interpolate target color
        this._cursorMat.AlbedoColor = this._cursorMat.AlbedoColor.Lerp(
            this._targetColor,
            (float)delta * 5f
        );
    }
}
