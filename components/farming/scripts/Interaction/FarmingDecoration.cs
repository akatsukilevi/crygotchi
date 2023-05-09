namespace Crygotchi;

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

        //* Should open the popup here
        tower.OpenInteraction(root);
    }
}
