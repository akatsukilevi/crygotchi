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

    public virtual void Deserialize(Dictionary<string, Variant> data, TilesDatabase tDB, ItemsDatabase iDB)
    {
        //* STUB implementation, will do nothing for standard static decorations
        //* ID isn't being imported since it is set by the `CreateInstance` call
    }
}
