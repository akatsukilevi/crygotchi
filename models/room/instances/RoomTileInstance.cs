using Godot.Collections;

namespace Crygotchi;

public partial class RoomTileInstance : Resource
{
    public string ID { get; set; }
    public Vector2 Position { get; set; }

    public RoomTile TileEntry { get; set; }

#nullable enable
    public RoomTileDecorationInstance? Decoration { get; set; } = null;
#nullable disable

    public Dictionary<string, Variant> Serialize()
    {
        return new()
        {
            { "ID", this.ID },
            { "Position", $"{this.Position.X},{this.Position.Y}" },
            { "Decoration", this.Decoration?.Serialize() },
        };
    }

    public static RoomTileInstance Deserialize(Dictionary<string, Variant> data, TilesDatabase tDB, ItemsDatabase iDB)
    {
        var decorationData = (Dictionary<string, Variant>)data["Decoration"];
        var decorationInstance = (RoomTileDecorationInstance)null;

        if (data.ContainsKey("Decoration") && decorationData?.ContainsKey("ID") == true)
        {
            var decoration = tDB.GetDecorationById((string)decorationData["ID"]);
            decorationInstance = decoration.CreateInstance();
            decorationInstance.Deserialize(decorationData, tDB, iDB);
        }

        var position = ((string)data["Position"]).Split(",");
        var instance = new RoomTileInstance()
        {
            ID = (string)data["ID"],
            Position = new Vector2(float.Parse(position[0]), float.Parse(position[1])),
            Decoration = decorationInstance
        };

        instance.TileEntry = tDB.GetTileById((string)data["ID"]);
        return instance;
    }
}
