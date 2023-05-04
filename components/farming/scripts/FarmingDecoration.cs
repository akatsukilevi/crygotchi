namespace Crygotchi;

public partial class FarmingDecoration : RoomTileDecoration
{
    public override bool IsInteractable => true;

    private CursorState _cursorState;

    public override RoomTileDecorationInstance CreateInstance()
    {
        return new FarmingInstance()
        {
            DecorationEntry = this,
            ID = this._id,
        };
    }

    public override void Interact(RoomTileDecorationInstance instance, Node source)
    {
        var farm = (FarmingInstance)instance;

        //* Get the cursor state if it is missing
        this._cursorState ??= source.GetNode<CursorState>("/root/CursorState");

        if (this._cursorState.IsHoldingItem())
        {
            farm.CreateLayer();
            return;
        }

        farm.RemoveLayer(0);
    }
}
