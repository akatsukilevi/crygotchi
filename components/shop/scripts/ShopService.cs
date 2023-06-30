namespace Crygotchi;

public partial class ShopService : Node
{
    [ExportCategory("References")]
    [Export] public PackedScene _windowTemplate;

    public override void _Ready()
    {
        base._Ready();
    }

    public ShopPopup OpenShop(IDatabaseItem[] items, IDatabaseItem[] owned, Action<IDatabaseItem> OnSelect, Action OnClose)
    {
        var shop = this._windowTemplate.Instantiate<ShopPopup>();
        shop.Ready += () => shop.Setup(items, owned, OnSelect, OnClose);
        this.AddChild(shop);

        return shop;
    }
}
