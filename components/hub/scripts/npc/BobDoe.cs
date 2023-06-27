namespace Crygotchi;

public partial class BobDoe : NPC
{
    private ShopService _shop;

    public override void _Ready()
    {
        base._Ready();
        this._shop = this.GetNode<ShopService>("/root/ShopService");
    }

    protected override void StartInteraction()
    {
        this.FinishInteraction();
    }
}
