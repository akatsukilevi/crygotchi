namespace AfterlifeAdventures;

public partial class RoomTileObject : Node3D
{
    [Export] private CsgBox3D Mesh;
    [Export] private Node3D DecorationParent;

    private RoomTileInstance TileInstance;
    private RoomTile Tile;

    public void Setup(RoomTileInstance tile)
    {
        this.TileInstance = tile;
        this.Name = $"{tile.Position.X},{tile.Position.Y}";
        this.Position = new Vector3(tile.Position.X * 2, 0f, tile.Position.Y * 2);

        this.Tile = tile.TileEntry;
        this.SetupPreview(this.Tile, true);
    }

    public void SetupPreview(RoomTile tile, bool setupDecoration = false)
    {
        //* Setup the material
        this.Mesh.Material = new StandardMaterial3D()
        {
            AlbedoColor = tile?.Color ?? new Color(1, 0, 1, 1)
        };

        if (!setupDecoration || this.TileInstance.Decoration == null) return;

        //* Setup the decoration
        var deco = this.TileInstance.Decoration.DecorationEntry;
        if (deco == null) return;

        var decoInstance = deco.Mesh.Instantiate();
        this.DecorationParent.AddChild(decoInstance);
    }

    public RoomTileInstance GetInstance()
    {
        return TileInstance;
    }

    public RoomTileDecorationInstance GetDecorationInstance()
    {
        return this.TileInstance.Decoration;
    }
}
