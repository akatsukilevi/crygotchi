using Godot.Collections;

namespace AfterlifeAdventures;

public class Room
{
    public System.Collections.Generic.Dictionary<Vector2, RoomTileInstance> Tiles = new();
    public System.Collections.Generic.List<string> OwnedTiles = new();
    public System.Collections.Generic.List<string> OwnedDecorations = new();

    public Dictionary<string, Variant> Serialize()
    {
        var serializedTiles = new Dictionary<string, Dictionary<string, Variant>>();
        foreach (var (key, tile) in this.Tiles) serializedTiles.Add($"{key.X},{key.Y}", tile.Serialize());

        return new Dictionary<string, Variant>()
        {
            { "Tiles", serializedTiles },
            { "OwnedTiles", this.OwnedTiles.ToArray() },
            { "OwnedDecorations", this.OwnedDecorations.ToArray() }
        };
    }

    public void Deserialize(Dictionary<string, Variant> data, TilesDatabase tDB, ItemsDatabase iDB)
    {
        this.Tiles.Clear();

        var serializedTiles = (Dictionary<string, Dictionary<string, Variant>>)data["Tiles"];
        GD.Print($"[ ROOM ] Loading {serializedTiles.Count} tiles");
        foreach (var (key, tileData) in serializedTiles)
        {
            var keyLocal = key.Split(",");
            var pos = new Vector2(float.Parse(keyLocal[0]), float.Parse(keyLocal[1]));

            this.Tiles.Add(pos, RoomTileInstance.Deserialize(tileData, tDB, iDB));
        }

        var serializedOwnedTiles = (string[])data["OwnedTiles"];
        var serializedOwnedDecorations = (string[])data["OwnedDecorations"];

        this.OwnedTiles = serializedOwnedTiles.ToList();
        this.OwnedDecorations = serializedOwnedDecorations.ToList();
    }

    public void UnlockTile(RoomTile tile)
    {
        this.OwnedTiles.Add(tile.GetId());
    }
}

public enum RoomMode
{
    Exploring,
    Building,
    Decorating,
}
