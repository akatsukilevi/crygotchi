namespace AfterlifeAdventures;

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
    public event Action OnInteract;

    private TilesDatabase _tilesDatabase;
    private SaveGame _save;

    public override void _Ready()
    {
        base._Ready();

        this._tilesDatabase = this.GetNode<TilesDatabase>("/root/TilesDatabase");
        this._save = this.GetNode<SaveManager>("/root/SaveManager").GetSave();

        this._selectedBuilding = this._tilesDatabase.GetUnlockedTileByIndex(0);
        this._selectedDecorating = this._tilesDatabase.GetUnlockedDecorationByIndex(0);

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

    public RoomTileInstance GetTileAt(Vector2 position)
    {
        return this._tiles.ContainsKey(position) ? this._tiles[position] : null;
    }

    public KeyValuePair<Vector2, RoomTileInstance>? GetRandomTile()
    {
        if (this._tiles.Count == 0) return null;
        return this._tiles.ElementAt(Random.Shared.Next(0, this._tiles.Count - 1));
    }

    public OSC[] GetInput(Vector2 cursorPosition)
    {
        switch (this._mode)
        {
            case RoomMode.Building:
                return this.GetBuildingModeInput(cursorPosition);
            case RoomMode.Decorating:
                return this.GetDecorationModeInput(cursorPosition);
            default:
                return this.GetExploringModeInput(cursorPosition);
        }
    }

    public void NotifyUpdate() => this.OnStateChange?.Invoke(false);

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
    #endregion

    #region "Exploring Mode"
    private OSC[] GetExploringModeInput(Vector2 cursorPosition)
    {
        var currentHovering = this.GetTileAt(cursorPosition);
        var input = new List<OSC>()
        {
            new OSC()
            {
                OnActivate = () => this.SetMode(RoomMode.Building),
                Name = "Switch to Building Mode",
                Key = OSCKey.Tertiary,
            }
        };

        if (currentHovering?.Decoration?.DecorationEntry?.IsInteractable == true)
        {
            input.Add(new OSC()
            {
                OnActivate = () =>
                {
                    this.GetNode<OSCController>("/root/OSCController").ClearOSC();
                    this.OnInteract?.Invoke();
                },
                Name = "Interact",
                Key = OSCKey.Primary,
            });
        }

        return input.ToArray();
    }
    #endregion

    #region "Building Mode"
    private OSC[] GetBuildingModeInput(Vector2 cursorPosition)
    {
        var currentHovering = this.GetTileAt(cursorPosition);
        var input = new List<OSC>()
        {
            new OSC()
            {
                OnActivate = () => this.SetMode(RoomMode.Decorating),
                Name = "Switch to Decorating Mode",
                Key = OSCKey.Tertiary,
            },
            new OSC()
            {
                OnActivate = this.NextSelectedBuilding,
                Name = "Next Tile",
                Key = OSCKey.ShiftRight,
            },
            new OSC()
            {
                OnActivate = this.PreviousSelectedBuilding,
                Name = "Previous Tile",
                Key = OSCKey.ShiftLeft,
            }
        };

        if (currentHovering != null)
        {
            input.Add(new OSC()
            {
                OnActivate = () => this.OnInteract?.Invoke(),
                Name = "Remove Tile",
                Key = OSCKey.Primary,
            });
        }
        else if (this._selectedBuilding != null)
        {
            input.Add(new OSC()
            {
                OnActivate = () => this.OnInteract?.Invoke(),
                Name = "Build Tile",
                Key = OSCKey.Primary,
            });
        }

        return input.ToArray();
    }

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
        this._selectedBuildingIndex = this._tilesDatabase.ClampUnlockedTileIndex(this._selectedBuildingIndex + 1);
        this._selectedBuilding = this._tilesDatabase.GetUnlockedTileByIndex(this._selectedBuildingIndex);

        this.OnStateChange?.Invoke(false);
    }

    public void PreviousSelectedBuilding()
    {
        this._selectedBuildingIndex = this._tilesDatabase.ClampUnlockedTileIndex(this._selectedBuildingIndex - 1);
        this._selectedBuilding = this._tilesDatabase.GetUnlockedTileByIndex(this._selectedBuildingIndex);

        this.OnStateChange?.Invoke(false);
    }
    #endregion

    #region "Decorating Mode"
    private OSC[] GetDecorationModeInput(Vector2 cursorPosition)
    {
        var currentHovering = this.GetTileAt(cursorPosition);
        var input = new List<OSC>()
        {
            new OSC()
            {
                OnActivate = () => this.SetMode(RoomMode.Exploring),
                Name = "Switch to Exploring Mode",
                Key = OSCKey.Tertiary,
            },
            new OSC()
            {
                OnActivate = this.NextSelectedDecorating,
                Name = "Next Decoration",
                Key = OSCKey.ShiftRight,
            },
            new OSC()
            {
                OnActivate = this.PreviousSelectedDecorating,
                Name = "Previous Decoration",
                Key = OSCKey.ShiftLeft,
            }
        };

        if (currentHovering == null) return input.ToArray();

        if (currentHovering.Decoration != null)
        {
            input.Add(new OSC()
            {
                OnActivate = () => this.OnInteract?.Invoke(),
                Name = "Remove Decoration",
                Key = OSCKey.Primary,
            });
        }
        else if (this._selectedDecorating != null)
        {
            input.Add(new OSC()
            {
                OnActivate = () => this.OnInteract?.Invoke(),
                Name = "Place Decoration",
                Key = OSCKey.Primary,
            });
        }

        return input.ToArray();
    }

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
        this._selectedDecoratingIndex = this._tilesDatabase.ClampUnlockedDecorationIndex(this._selectedDecoratingIndex + 1);
        this._selectedDecorating = this._tilesDatabase.GetUnlockedDecorationByIndex(this._selectedDecoratingIndex);

        this.OnStateChange?.Invoke(false);
    }

    public void PreviousSelectedDecorating()
    {
        this._selectedDecoratingIndex = this._tilesDatabase.ClampUnlockedDecorationIndex(this._selectedDecoratingIndex - 1);
        this._selectedDecorating = this._tilesDatabase.GetUnlockedDecorationByIndex(this._selectedDecoratingIndex);

        this.OnStateChange?.Invoke(false);
    }
    #endregion
}
