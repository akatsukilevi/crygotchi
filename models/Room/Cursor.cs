using Godot.Collections;

namespace Crygotchi;

public class Cursor
{
    public Vector2 Position = Vector2.Zero;
    public string HeldItemID = null;

    public Dictionary<string, Variant> Serialize()
    {
        return new Dictionary<string, Variant>()
        {
            { "Position", $"{this.Position.X},{this.Position.Y}" },
            { "HeldItemID", this.HeldItemID },
        };
    }

    public void Deserialize(Dictionary<string, Variant> data)
    {
        var position = ((string)data["Position"]).Split(",");
        this.Position = new Vector2(float.Parse(position[0]), float.Parse(position[1]));
        var ItemID = (string)data["HeldItemID"];
        this.HeldItemID = ItemID == "" ? null : ItemID;
    }
}
