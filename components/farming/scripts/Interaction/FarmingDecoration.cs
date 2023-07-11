namespace AfterlifeAdventures;

public partial class FarmingDecoration : RoomTileDecoration
{
    public override bool IsInteractable => true;

    private CursorState _cursorState;

    public override RoomTileDecorationInstance CreateInstance()
    {
        var instance = new FarmingInstance()
        {
            DecorationEntry = this,
            ID = this._id,
        };

        return instance;
    }

    public override void Interact(RoomTileDecorationInstance instance, Node source)
    {
        var farm = (FarmingInstance)instance;
        var root = source.GetTree().Root;
        var tower = farm.GetTower();

        //* Get the cursor state if it is missing
        this._cursorState ??= source.GetNode<CursorState>("/root/CursorState");

        //* Is cursor holding something?
        if (this._cursorState.IsHoldingItem() && this._cursorState.ItemTypeCheck<SeedItem>())
        {
            //* If yes, add to the inventory and remove from cursor
            GD.Print("Cursor is holding a seed and opened farm, transferring seeds");

            var item = (SeedItem)this._cursorState.TakeItem();
            GD.Print($"Got seed {item} from cursor");

            farm.AddSeed(item);
            return;
        }

        //* Should open the popup here
        tower.OpenInteraction(root);
    }
}
