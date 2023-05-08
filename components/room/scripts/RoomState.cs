namespace Crygotchi;

using System.Collections.Generic;

public partial class RoomState : Node
{
    private readonly Dictionary<string, RoomTileInstance> _tiles = new();
    private RoomMode _mode = RoomMode.Exploring;

    private RoomTile _selectedBuilding = null;
    private int _selectedBuildingIndex = 0;

    private RoomTileDecoration _selectedDecorating = null;
    private int _selectedDecoratingIndex = 0;

    public event Action OnStateChange;
    private TilesDatabase _tilesDatabase;

    public override void _Ready()
    {
        base._Ready();

        this._tilesDatabase = this.GetNode<TilesDatabase>("/root/TilesDatabase");
        this._selectedBuilding = this._tilesDatabase.GetTileByIndex(0);
        this._selectedDecorating = this._tilesDatabase.GetDecorationByIndex(0);
    }

    #region "General"

    public RoomMode GetMode()
    {
        return this._mode;
    }

    public void SetMode(RoomMode newMode)
    {
        this._mode = newMode;
        this.OnStateChange?.Invoke();
    }

    public RoomTileInstance GetTileAt(Vector2 position) =>
        !this._tiles.ContainsKey($"{position.X},{position.Y}") ?
            null : this._tiles[$"{position.X},{position.Y}"];

    public void NotifyUpdate() => this.OnStateChange?.Invoke();
    #endregion

    #region "Building Mode"

    public RoomTile GetSelectedBuilding()
    {
        return this._selectedBuilding;
    }

    public void SetSelectedBuilding(RoomTile newSelected)
    {
        this._selectedBuilding = newSelected;
        this.OnStateChange?.Invoke();
    }

    public RoomTileInstance PutTileAtPosition(Vector2 position)
    {
        //* Do not try to put a tile if already exists
        if (this._tiles.ContainsKey($"{position.X},{position.Y}")) return null;

        var tile = new RoomTileInstance()
        {
            ID = this._selectedBuilding.GetId(),
            Position = position,
            TileEntry = this._selectedBuilding,
        };

        this._tiles[$"{position.X},{position.Y}"] = tile;
        this.OnStateChange?.Invoke();
        return tile;
    }

    public void RemoveTileAtPosition(Vector2 position)
    {
        if (!this._tiles.ContainsKey($"{position.X},{position.Y}")) return;

        var toRemove = this._tiles[$"{position.X},{position.Y}"];
        this._tiles.Remove($"{position.X},{position.Y}");
        toRemove.Dispose();

        this.OnStateChange?.Invoke();
    }

    public void NextSelectedBuilding()
    {
        this._selectedBuildingIndex = this._tilesDatabase.ClampTileIndex(this._selectedBuildingIndex + 1);
        this._selectedBuilding = this._tilesDatabase.GetTileByIndex(this._selectedBuildingIndex);

        this.OnStateChange?.Invoke();
    }

    public void PreviousSelectedBuilding()
    {
        this._selectedBuildingIndex = this._tilesDatabase.ClampTileIndex(this._selectedBuildingIndex - 1);
        this._selectedBuilding = this._tilesDatabase.GetTileByIndex(this._selectedBuildingIndex);

        this.OnStateChange?.Invoke();
    }
    #endregion

    #region "Decorating Mode"
    public RoomTileDecoration GetSelectedDecorating()
    {
        return this._selectedDecorating;
    }

    public void SetSelectedDecorating(RoomTileDecoration newSelected)
    {
        this._selectedDecorating = newSelected;
        this.OnStateChange?.Invoke();
    }

    public void NextSelectedDecorating()
    {
        this._selectedDecoratingIndex = this._tilesDatabase.ClampDecorationIndex(this._selectedDecoratingIndex + 1);
        this._selectedDecorating = this._tilesDatabase.GetDecorationByIndex(this._selectedDecoratingIndex);

        this.OnStateChange?.Invoke();
    }

    public void PreviousSelectedDecorating()
    {
        this._selectedDecoratingIndex = this._tilesDatabase.ClampDecorationIndex(this._selectedDecoratingIndex - 1);
        this._selectedDecorating = this._tilesDatabase.GetDecorationByIndex(this._selectedDecoratingIndex);

        this.OnStateChange?.Invoke();
    }
    #endregion
}

public enum RoomMode
{
    Exploring,
    Building,
    Decorating,
}
