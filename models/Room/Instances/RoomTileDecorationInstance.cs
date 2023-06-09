using Godot.Collections;

namespace Crygotchi;

public abstract partial class RoomTileDecorationInstance : Resource
{
    public string ID { get; set; }

    public RoomTileDecoration DecorationEntry { get; set; }

    public virtual Dictionary<string, Variant> Serialize()
    {
        return new()
        {
            { "ID", this.ID },
        };
    }
}
