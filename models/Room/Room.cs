using Godot.Collections;

namespace Crygotchi;

public class Room
{
    public System.Collections.Generic.Dictionary<Vector2, RoomTileInstance> Tiles = new();

    public Dictionary<string, Variant> Serialize()
    {
        var serializedTiles = new Dictionary<string, Dictionary<string, Variant>>();
        foreach (var (key, tile) in this.Tiles) serializedTiles.Add($"{key.X},{key.Y}", tile.Serialize());

        return new Dictionary<string, Variant>()
        {
            { "Tiles", serializedTiles },
        };
    }

    public void Deserialize(Dictionary<string, Variant> data, TilesDatabase tDB, ItemsDatabase iDB)
    {
        this.Tiles.Clear();

        var serializedTiles = (Dictionary<string, Dictionary<string, Variant>>)data["Tiles"];
        foreach (var (key, tileData) in serializedTiles)
        {
            GD.Print($"[ ROOM ] Loading tile {key}");
            var keyLocal = key.Split(",");
            var pos = new Vector2(float.Parse(keyLocal[0]), float.Parse(keyLocal[1]));

            this.Tiles.Add(pos, RoomTileInstance.Deserialize(tileData, tDB, iDB));
        }
    }
}

public enum RoomMode
{
    Exploring,
    Building,
    Decorating,
}
