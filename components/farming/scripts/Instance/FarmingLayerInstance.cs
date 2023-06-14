using Godot.Collections;

namespace Crygotchi;

public partial class FarmingLayerInstance : Resource
{
    public FarmingSlot FirstSlot;
    public FarmingSlot SecondSlot;
    public FarmingSlot ThirdSlot;
    public FarmingSlot FourthSlot;

    public FarmingLayerInstance()
    {
        this.FirstSlot = new();
        this.SecondSlot = new();
        this.ThirdSlot = new();
        this.FourthSlot = new();
    }

    public Dictionary<string, Variant> Serialize()
    {
        return new()
        {
            { "FirstSlot", this.FirstSlot.Serialize() },
            { "SecondSlot", this.SecondSlot.Serialize() },
            { "ThirdSlot", this.ThirdSlot.Serialize() },
            { "FourthSlot", this.FourthSlot.Serialize() },
        };
    }

    public static FarmingLayerInstance Deserialize(Dictionary<string, Variant> data)
    {
        var layer = new FarmingLayerInstance();

        layer.FirstSlot.Deserialize((Dictionary<string, Variant>)data["FirstSlot"]);
        layer.SecondSlot.Deserialize((Dictionary<string, Variant>)data["SecondSlot"]);
        layer.ThirdSlot.Deserialize((Dictionary<string, Variant>)data["ThirdSlot"]);
        layer.FourthSlot.Deserialize((Dictionary<string, Variant>)data["FourthSlot"]);

        return layer;
    }
}
