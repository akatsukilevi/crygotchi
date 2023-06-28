namespace Crygotchi;

public abstract partial class RoomTileDecoration : Resource, IDatabaseItem
{
    [ExportCategory("Metadata")]
    [Export] public string Name { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture2D Icon { get; set; }

    [ExportCategory("Shop Settings")]
    [Export] public bool Purchasable { get; set; }
    [Export] public int Cost { get; set; }

    [ExportCategory("World")]
    [ExportGroup("Visual")]
    [Export] public PackedScene Mesh;

    public abstract bool IsInteractable { get; }
    protected string _id;

    public virtual RoomTileDecoration Setup(string id)
    {
        this._id = id;
        return this;
    }

    public virtual RoomTileDecorationInstance CreateInstance()
    {
        return new RoomTileBaseDecorationInstance() { ID = this._id, DecorationEntry = this };
    }

    public virtual void Interact(RoomTileDecorationInstance instance, Node source)
    {
        //* Stub implementation
    }

    public string GetId()
    {
        return this._id;
    }
}
