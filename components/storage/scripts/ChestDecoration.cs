namespace AfterlifeAdventures;

public partial class ChestDecoration : RoomTileDecoration
{
    public override bool IsInteractable => true;

    private CursorState _cursorState;

    public override RoomTileDecorationInstance CreateInstance()
    {
        return new ChestDecorationInstance()
        {
            DecorationEntry = this,
            ID = this._id,
        };
    }

    public override void Interact(RoomTileDecorationInstance instance, Node source)
    {
        var chest = (ChestDecorationInstance)instance;

        //* Get the cursor state if it is missing
        this._cursorState ??= source.GetNode<CursorState>("/root/CursorState");

        //* Is cursor holding something?
        if (this._cursorState.IsHoldingItem())
        {
            //* If yes, add to the inventory and remove from cursor
            GD.Print("Cursor is holding a item and opened storage, transferring item");

            var item = this._cursorState.TakeItem();
            GD.Print($"Got item {item} from cursor");

            chest.AddItem(item);
            return;
        }

        //* If no, open the inventory UIs
        GD.Print("Cursor is not holding a item and opened storage, opening popup");
        var popup = GD.Load<PackedScene>("res://components/storage/ui/storage_popup.tscn").Instantiate<StoragePopup>();
        var root = source.GetTree().Root;
        popup.Ready += () => popup.Setup(chest);

        root.AddChild(popup);
    }
}
