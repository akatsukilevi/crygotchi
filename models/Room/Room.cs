using Godot.Collections;

namespace Crygotchi;

public class Room
{
    public System.Collections.Generic.Dictionary<Vector2, RoomTileInstance> Tiles = new();
    public RoomMode Mode = RoomMode.Exploring;

    public Dictionary<string, Variant> Serialize()
    {
        var serializedTiles = new Dictionary<Vector2, Dictionary<string, Variant>>();
        foreach (var (key, tile) in this.Tiles) serializedTiles.Add(key, tile.Serialize());

        return new Dictionary<string, Variant>()
        {
            { "Mode", (int)this.Mode },
            { "Tiles", serializedTiles },
        };
    }
}

public enum RoomMode
{
    Exploring,
    Building,
    Decorating,
}
