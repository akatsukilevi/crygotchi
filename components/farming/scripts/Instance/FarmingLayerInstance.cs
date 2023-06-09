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

    public Godot.Collections.Dictionary<string, Variant> Serialize()
    {
        return new()
        {
            { "FirstSlot", this.FirstSlot.Serialize() },
            { "SecondSlot", this.SecondSlot.Serialize() },
            { "ThirdSlot", this.ThirdSlot.Serialize() },
            { "FourthSlot", this.FourthSlot.Serialize() },
        };
    }
}
