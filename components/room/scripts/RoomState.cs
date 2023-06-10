namespace Crygotchi;

using System.Collections.Generic;

public partial class RoomState : Node
{
    private readonly Dictionary<Vector2, RoomTileInstance> _tiles = new();
    private RoomMode _mode = RoomMode.Exploring;

    private RoomTile _selectedBuilding = null;
    private int _selectedBuildingIndex = 0;

    private RoomTileDecoration _selectedDecorating = null;
    private int _selectedDecoratingIndex = 0;

    public event Action<bool> OnStateChange;

    private TilesDatabase _tilesDatabase;
    private SaveGame _save;

    public override void _Ready()
    {
        base._Ready();

        this._tilesDatabase = this.GetNode<TilesDatabase>("/root/TilesDatabase");
        this._save = this.GetNode<SaveManager>("/root/SaveManager").GetSave();

        this._selectedBuilding = this._tilesDatabase.GetTileByIndex(0);
        this._selectedDecorating = this._tilesDatabase.GetDecorationByIndex(0);

        this._save.OnSaveUpdated += this.OnSaveUpdated;
    }

    #region "General"

    public RoomMode GetMode()
    {
        return this._mode;
    }

    public Dictionary<Vector2, RoomTileInstance> GetTiles()
    {
        return this._tiles;
    }

    public void SetMode(RoomMode newMode)
    {
        this._mode = newMode;
        this.OnStateChange?.Invoke(false);
    }

    public RoomTileInstance GetTileAt(Vector2 position) =>
        !this._tiles.ContainsKey(position) ?
            null : this._tiles[position];

    public void NotifyUpdate() => this.OnStateChange?.Invoke(false);
    #endregion

    #region "Building Mode"

    public RoomTile GetSelectedBuilding()
    {
        return this._selectedBuilding;
    }

    public void SetSelectedBuilding(RoomTile newSelected)
    {
        this._selectedBuilding = newSelected;
        this.OnStateChange?.Invoke(false);
    }

    public RoomTileInstance PutTileAtPosition(Vector2 position)
    {
        //* Do not try to put a tile if already exists
        if (this._tiles.ContainsKey(position)) return null;

        var tile = new RoomTileInstance()
        {
            ID = this._selectedBuilding.GetId(),
            Position = position,
            TileEntry = this._selectedBuilding,
        };

        this._tiles[position] = tile;
        this.OnStateChange?.Invoke(false);
        return tile;
    }

    public void RemoveTileAtPosition(Vector2 position)
    {
        if (!this._tiles.ContainsKey(position)) return;

        var toRemove = this._tiles[position];
        this._tiles.Remove(position);
        toRemove.Dispose();

        this.OnStateChange?.Invoke(false);
    }

    public void NextSelectedBuilding()
    {
        this._selectedBuildingIndex = this._tilesDatabase.ClampTileIndex(this._selectedBuildingIndex + 1);
        this._selectedBuilding = this._tilesDatabase.GetTileByIndex(this._selectedBuildingIndex);

        this.OnStateChange?.Invoke(false);
    }

    public void PreviousSelectedBuilding()
    {
        this._selectedBuildingIndex = this._tilesDatabase.ClampTileIndex(this._selectedBuildingIndex - 1);
        this._selectedBuilding = this._tilesDatabase.GetTileByIndex(this._selectedBuildingIndex);

        this.OnStateChange?.Invoke(false);
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
        this.OnStateChange?.Invoke(false);
    }

    public void NextSelectedDecorating()
    {
        this._selectedDecoratingIndex = this._tilesDatabase.ClampDecorationIndex(this._selectedDecoratingIndex + 1);
        this._selectedDecorating = this._tilesDatabase.GetDecorationByIndex(this._selectedDecoratingIndex);

        this.OnStateChange?.Invoke(false);
    }

    public void PreviousSelectedDecorating()
    {
        this._selectedDecoratingIndex = this._tilesDatabase.ClampDecorationIndex(this._selectedDecoratingIndex - 1);
        this._selectedDecorating = this._tilesDatabase.GetDecorationByIndex(this._selectedDecoratingIndex);

        this.OnStateChange?.Invoke(false);
    }
    #endregion

    private void OnSaveUpdated()
    {
        var state = this._save.GetRoomSaveState();

        this._tiles.Clear();
        foreach (var (k, v) in state.Tiles)
        {
            //* Should somehow tell the manager "hey, missing stuff here"
            this._tiles.Add(k, v);
        }

        this.OnStateChange?.Invoke(true);
    }
}
