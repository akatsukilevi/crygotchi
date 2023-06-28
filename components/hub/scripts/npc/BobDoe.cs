namespace Crygotchi;

public partial class BobDoe : NPC
{
    private ShopService _shop;
    private TilesDatabase _tiles;

    public override void _Ready()
    {
        base._Ready();
        this._shop = this.GetNode<ShopService>("/root/ShopService");
        this._tiles = this.GetNode<TilesDatabase>("/root/TilesDatabase");
    }

    protected override void StartInteraction()
    {
        this._shop.OpenShop(this._tiles.GetShopTiles().ToArray(), () => this.FinishInteraction());
    }
}
