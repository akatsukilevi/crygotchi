namespace Crygotchi;

public partial class RoomTile : Resource
{
    [ExportCategory("Metadata")]
    [Export] public string Name;
    [Export] public string Description;
    [Export] public Texture2D Icon;

    [ExportCategory("Store")]
    [Export] public bool Purchasable = true;
    [Export] public int Cost = 10;

    [ExportCategory("Visual")]
    [Export] public Color Color = new() { R = 1, G = 1, B = 1, A = 1 };

    private string _id;

    public RoomTile Setup(string id)
    {
        this._id = id;
        return this;
    }

    public string GetId()
    {
        return this._id;
    }
}
