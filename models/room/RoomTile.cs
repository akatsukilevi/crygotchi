namespace AfterlifeAdventures;

public partial class RoomTile : Resource, IDatabaseItem
{
    [ExportCategory("Metadata")]
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture2D Icon { get; set; }

    [ExportCategory("Shop Settings")]
    [Export] public bool Purchasable { get; set; }
    [Export] public int Cost { get; set; }

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
