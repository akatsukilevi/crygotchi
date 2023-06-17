namespace Crygotchi;

using System.Collections.Generic;

public partial class TilesDatabase : Node
{
    private Dictionary<string, RoomTile> _tiles;
    private Dictionary<string, RoomTileDecoration> _decorations;

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

    public int ClampTileIndex(int number)
    {
        int amount = this._tiles.Count;

        if (number >= amount)
            return 0;

        return number < 0 ? amount - 1 : number;
    }

    public int ClampDecorationIndex(int number)
    {
        int amount = this._decorations.Count;

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

    public RoomTileDecoration GetDecorationById(string id)
    {
        if (this._decorations.TryGetValue(id, out RoomTileDecoration decoration)) return decoration;

        GD.PushWarning($"Cannot find decoration \"{id}\"");
        return null;
    }

    public RoomTile GetTileByIndex(int index)
    {
        return this._tiles.ElementAt(index).Value;
    }

    public RoomTileDecoration GetDecorationByIndex(int index)
    {
        return this._decorations.ElementAt(index).Value;
    }
}
