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
            { "Position", this.Position },
            { "HeldItemID", this.HeldItemID },
        };
    }
}
