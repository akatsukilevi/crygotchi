namespace Crygotchi;

public partial class ShopPopup : Panel
{
    [ExportCategory("References")]
    [ExportGroup("List")]
    [Export] public ItemList Items;
    [ExportGroup("Item")]
    [Export] public Node Content;
    [Export] public Label ItemTitle;
    [Export] public Label ItemPrice;
    [Export] public TextureRect ItemIcon;
    [Export] public RichTextLabel ItemDescription;

    private bool _hasSetup = false;
    private OSCController _osc;

    public override void _Ready()
    {
        base._Ready();
        this._osc = this.GetNode<OSCController>("/root/OSCController");
    }

    public void Setup()
    {
        this._osc.RegisterOSC(new OSC[] {
            new() {
                Key = OSCKey.Cancel,
                Name = "Close",
                OnActivate = this.Close
            }
        });

        this._hasSetup = true;
    }

    private void Close()
    {
        this.QueueFree();
    }
}
