namespace Crygotchi;

public partial class ShopService : Node
{
    [ExportCategory("References")]
    [Export] public PackedScene _windowTemplate;

    public override void _Ready()
    {
        base._Ready();
    }

    public void OpenShop(IDatabaseItem[] items, Action OnClose)
    {
        var child = this._windowTemplate.Instantiate<ShopPopup>();
        child.Ready += () => child.Setup(items, OnClose);
        this.AddChild(child);
    }
}
