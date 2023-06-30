namespace Crygotchi;

using System.Collections.Generic;

public partial class TilesDatabase : Node
{
    private Dictionary<string, RoomTileDecoration> _decorations;
    private Dictionary<string, RoomTile> _tiles;

    private SaveGame _save;
    private List<string> _unlockedTiles = new();
    private List<string> _unlockedDecorations = new();

    public TilesDatabase() : base()
    {
        var loadedTiles = FileSystemUtils.LoadAll<RoomTile>("res://resources/tiles/floors");
        var loadedDecorations = FileSystemUtils.LoadAll<RoomTileDecoration>("res://resources/tiles/decorations");
        this._tiles = new();
        this._decorations = new();

        foreach (var item in loadedTiles)
        {
            var id = ResourceUid.IdToText(ResourceLoader.GetResourceUid(item.ResourcePath));
            var path = item.ResourcePath;

            if (id.Contains("<invalid>"))
            {
                GD.PrintErr($"Invalid ID for tile {path}");
                continue;
            }

            if (this._tiles.ContainsKey(id))
            {
                GD.PrintErr($"Cannot add duplicated tile \"{id}\" ({path})");
                continue;
            }

            GD.Print($"Loading in tile \"{id}\" ({path})");
            this._tiles.Add(id, item.Setup(id));
        }

        foreach (var item in loadedDecorations)
        {
            var id = ResourceUid.IdToText(ResourceLoader.GetResourceUid(item.ResourcePath));
            var path = item.ResourcePath;

            if (this._tiles.ContainsKey(id))
            {
                GD.PrintErr($"Cannot add duplicated decoration \"{id}\" ({path})");
                continue;
            }

            GD.Print($"Loading in decoration \"{id}\" ({path})");
            this._decorations.Add(id, item.Setup(id));
        }
    }

    public override void _Ready()
    {
        base._Ready();
        this._save = this.GetNode<SaveManager>("/root/SaveManager").GetSave();
        this._save.OnSaveUpdated += this.OnSaveUpdated;
    }

    private void OnSaveUpdated()
    {
        var save = this._save.GetRoomSaveState();
        this._unlockedTiles = save.OwnedTiles;
        this._unlockedDecorations = save.OwnedDecorations;
    }

    public List<RoomTile> GetShopTiles()
    {
        return this._tiles.Values.ToList();
    }

    public List<RoomTile> GetShopOwnedTiles()
    {
        var tiles = new List<RoomTile>();
        foreach (var tile in this._unlockedTiles)
        {
            tiles.Add(this.GetTileById(tile));
        }

        return tiles;
    }

    public List<RoomTileDecoration> GetShopDecorations()
    {
        return this._decorations.Values.ToList();
    }

    public List<RoomTileDecoration> GetShopOwnedDecorations()
    {
        var decorations = new List<RoomTileDecoration>();
        foreach (var decoration in this._unlockedDecorations)
        {
            decorations.Add(this.GetDecorationById(decoration));
        }

        return decorations;
    }

    #region "Tiles"
    public int ClampTileIndex(int number)
    {
        int amount = this._tiles.Count;

        if (number >= amount)
            return 0;

        return number < 0 ? amount - 1 : number;
    }

    public int ClampUnlockedTileIndex(int number)
    {
        int amount = this._unlockedTiles.Count;

        if (number >= amount)
            return 0;

        return number < 0 ? amount - 1 : number;
    }

    public RoomTile GetTileById(string id)
    {
        if (this._tiles.TryGetValue(id, out RoomTile tile)) return tile;

        GD.PushWarning($"Cannot find tile \"{id}\"");
        return null;
    }

    public RoomTile GetUnlockedTileById(string id)
    {
        if (!this._unlockedTiles.Contains(id)) return null;
        if (this._tiles.TryGetValue(id, out RoomTile tile)) return tile;

        GD.PushWarning($"Cannot find tile \"{id}\"");
        return null;
    }

    public RoomTile GetTileByIndex(int index)
    {
        return this._tiles.ElementAt(index).Value;
    }

    public RoomTile GetUnlockedTileByIndex(int index)
    {
        if (this._unlockedTiles.Count < index) return null;
        if (this._unlockedTiles.Count == 0) return null;

        var id = this._unlockedTiles.ElementAt(index);
        return this.GetUnlockedTileById(id);
    }
    #endregion

    #region "Decorations"
    public int ClampDecorationIndex(int number)
    {
        int amount = this._decorations.Count;

        if (number >= amount)
            return 0;

        return number < 0 ? amount - 1 : number;
    }


    public int ClampUnlockedDecorationIndex(int number)
    {
        int amount = this._unlockedDecorations.Count;

        if (number >= amount)
            return 0;

        return number < 0 ? amount - 1 : number;
    }

    public RoomTileDecoration GetUnlockedDecorationById(string id)
    {
        if (!this._unlockedDecorations.Contains(id)) return null;
        if (this._decorations.TryGetValue(id, out RoomTileDecoration decoration)) return decoration;

        GD.PushWarning($"Cannot find decoration \"{id}\"");
        return null;
    }

    public RoomTileDecoration GetDecorationById(string id)
    {
        if (this._decorations.TryGetValue(id, out RoomTileDecoration decoration)) return decoration;

        GD.PushWarning($"Cannot find decoration \"{id}\"");
        return null;
    }

    public RoomTileDecoration GetDecorationByIndex(int index)
    {
        return this._decorations.ElementAt(index).Value;
    }

    public RoomTileDecoration GetUnlockedDecorationByIndex(int index)
    {
        if (this._unlockedDecorations.Count < index) return null;
        if (this._unlockedDecorations.Count == 0) return null;

        var id = this._unlockedDecorations.ElementAt(index);
        return this.GetUnlockedDecorationById(id);
    }
    #endregion
}
