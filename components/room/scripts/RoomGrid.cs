namespace Crygotchi;

using Godot.Collections;

public partial class RoomGrid : Node
{
    [ExportCategory("References")]
    [ExportGroup("Objects")]
    [ExportSubgroup("World")]
    [Export] private Node3D Cursor;
    [Export] private Node3D TilesList;
    [ExportSubgroup("UI")]
    [Export] private TextureRect MainIndicator;
    [Export] private TextureRect SubIndicator;
    [Export] private Label TileIndicator;
    [ExportGroup("Templates")]
    [Export] private PackedScene TileTemplate;
    [ExportGroup("Assets")]
    [Export] private Texture2D ExploringSprite;
    [Export] private Texture2D BuildingSprite;
    [Export] private Texture2D DecoratingSprite;

    private Dictionary<string, RoomTileObject> _instances = new();
    private CursorState _cursorState;
    private RoomState _roomState;

    public override void _Ready()
    {
        base._Ready();

        this._cursorState = this.GetNode<CursorState>("/root/CursorState");
        this._roomState = this.GetNode<RoomState>("/root/RoomState");

        this._roomState.OnStateChange += this.OnStateChange;
        this._cursorState.OnAction += this.OnCursorAction;

        this.OnStateChange(false);
    }

    #region "General"
    public void SwitchMode()
    {
        switch (this._roomState.GetMode())
        {
            case RoomMode.Exploring:
                this._roomState.SetMode(RoomMode.Building);
                break;
            case RoomMode.Building:
                this._roomState.SetMode(RoomMode.Decorating);
                break;
            case RoomMode.Decorating:
                this._roomState.SetMode(RoomMode.Exploring);
                break;
        }
    }

    private void OnCursorAction(object sender, CursorActionEventArgs e)
    {
        if (e.Action != ActionType.Primary) return;

        switch (this._roomState.GetMode())
        {
            case RoomMode.Exploring:
                this.InteractTile();
                break;
            case RoomMode.Building:
                this.PutTile();
                break;
            case RoomMode.Decorating:
                this.PutDecoration();
                break;
        }
    }

    private void OnStateChange(bool shouldResyncWithState)
    {
        if (shouldResyncWithState)
        {
            foreach (var instance in this._instances) instance.Value.QueueFree();
            this._instances.Clear();

            foreach (var (position, tile) in this._roomState.GetTiles()) this.InstantiateTile(position, tile);
        }

        switch (this._roomState.GetMode())
        {
            case RoomMode.Exploring:
                this.MainIndicator.Texture = this.ExploringSprite;
                this.SubIndicator.Texture = null;
                this.TileIndicator.Text = "";
                break;
            case RoomMode.Building:
                this.MainIndicator.Texture = this.BuildingSprite;
                this.SubIndicator.Texture = this._roomState.GetSelectedBuilding()?.Icon;
                this.TileIndicator.Text = this._roomState.GetSelectedBuilding()?.Name;
                break;
            case RoomMode.Decorating:
                this.MainIndicator.Texture = this.DecoratingSprite;
                this.SubIndicator.Texture = this._roomState.GetSelectedDecorating()?.Icon;
                this.TileIndicator.Text = this._roomState.GetSelectedDecorating()?.Name;
                break;
        }
    }
    #endregion

    #region "Exploring mode"
    public void InteractTile()
    {
        var position = this._cursorState.GetPosition();
        var currentHovering = this._roomState.GetTileAt(position);
        var currentDecoration = currentHovering?.Decoration;

        if (currentHovering == null || currentDecoration == null) return; //* Either no tile or no decoration

        var decoration = currentDecoration.DecorationEntry ??
            throw new Exception($"Can't find decoration \"{currentDecoration.ID}\"!");

        if (!decoration.IsInteractable)
            return; //* Has no interaction

        //* Can interact here
        decoration.Interact(currentDecoration, this);
    }
    #endregion

    #region "Building mode"
    public void PutTile()
    {
        var position = this._cursorState.GetPosition();
        var key = $"{position.X},{position.Y}";
        var currentHovering = this._roomState.GetTileAt(position);

        //* Shold remove it if not null
        if (currentHovering != null)
        {
            this.DeleteTileInstance(position);
            return;
        }

        var tile = this._roomState.PutTileAtPosition(position);
        if (tile == null) return;

        this.InstantiateTile(position, tile);
    }

    public void DeleteTileInstance(Vector2 position, bool removeTileFromState = true)
    {
        var key = $"{position.X},{position.Y}";
        var child = this._instances[key];

        if (removeTileFromState) this._roomState.RemoveTileAtPosition(position);
        child.QueueFree();
        this._instances.Remove(key);
    }

    public void InstantiateTile(Vector2 position, RoomTileInstance tile)
    {
        var key = $"{position.X},{position.Y}";
        var tileObject = this.TileTemplate.Instantiate<RoomTileObject>();
        tileObject.Ready += () => tileObject.Setup(tile);
        this._instances[key] = tileObject;

        this.TilesList.AddChild(tileObject);
    }
    #endregion

    #region "Decoration mode"
    public void PutDecoration()
    {
        var position = this._cursorState.GetPosition();
        var currentHovering = this._roomState.GetTileAt(position);
        if (currentHovering == null) return;

        if (currentHovering.Decoration != null)
        {
            //* Already has a decoration, remove it instead
            currentHovering.Decoration = null;
            this.DeleteTileInstance(position, false);
            this.InstantiateTile(position, currentHovering);
            this._roomState.NotifyUpdate();
            return;
        }

        //* Add the current decoration into the hovering tile
        var currentDecoration = this._roomState.GetSelectedDecorating();
        currentHovering.Decoration = currentDecoration.CreateInstance();

        //* Should update the tile
        this.DeleteTileInstance(position, false);
        this.InstantiateTile(position, currentHovering);
        this._roomState.NotifyUpdate();
    }
    #endregion
}
