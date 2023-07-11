namespace AfterlifeAdventures;

public partial class RoomTileObjectDecoration : Node3D
{
    private RoomTileInstance TileInstance;
    private RoomTile Tile;

    public void Setup(RoomTileInstance tile)
    {
        this.TileInstance = tile;
        this.Name = $"{tile.Position.X},{tile.Position.Y}";
        this.Position = new Vector3(tile.Position.X * 2, 0f, tile.Position.Y * 2);

        this.Tile = this.TileInstance.TileEntry;
        this.SetupPreview(this.Tile);
    }

    public void SetupPreview(RoomTile tile)
    {
        // TODO: Preview for decorations
    }
}
