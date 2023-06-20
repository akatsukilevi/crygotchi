namespace Crygotchi;

public abstract partial class Item : Resource
{
    [ExportCategory("Metadata")]
    [Export] public string Name;
    [Export] public string Description;
    [Export] public Texture2D Icon;
    [Export] public PackedScene IconMesh;

    [ExportCategory("Store")]
    [Export] public bool Purchasable = true;
    [Export] public int Cost = 10;

    private string _id;

    public Item Setup(string id)
    {
        this._id = id;
        return this;
    }

    public string GetId()
    {
        return this._id;
    }
}
