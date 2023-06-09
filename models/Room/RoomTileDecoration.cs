namespace Crygotchi;

public abstract partial class RoomTileDecoration : Resource
{
    [ExportCategory("Metadata")]
    [Export] public string Name;
    [Export] public string Description;
    [Export] public Texture2D Icon;

    [ExportCategory("Store")]
    [Export] public bool Purchasable = true;
    [Export] public int Cost = 10;

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
