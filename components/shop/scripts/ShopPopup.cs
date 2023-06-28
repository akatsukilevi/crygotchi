using System.Collections.Generic;

namespace Crygotchi;

public partial class ShopPopup : Panel
{
    [ExportCategory("References")]
    [ExportGroup("List")]
    [Export] public ItemList Items;
    [ExportGroup("Item")]
    [Export] public Control ItemDetails;
    [Export] public Label ItemTitle;
    [Export] public Label ItemPrice;
    [Export] public TextureRect ItemIcon;
    [Export] public RichTextLabel ItemDescription;

    private OSCController _osc;

    private List<IDatabaseItem> _items = new();
    private Action _onClose;
    private bool _hasSetup = false;

    public override void _Ready()
    {
        base._Ready();

        this._osc = this.GetNode<OSCController>("/root/OSCController");
    }

    public void Setup(IDatabaseItem[] items, Action OnClose)
    {
        this._osc.RegisterOSC(new OSC[] {
            new() {
                Key = OSCKey.Cancel,
                Name = "Close",
                OnActivate = this.Close
            }
        });

        this._items = items.ToList();
        this._onClose = OnClose;

        this.Items.Clear();
        foreach (var item in items) this.Items.AddItem(item.Name, item.Icon);

        this.Items.EmptyClicked += OnEmptyClick;
        this.Items.ItemActivated += OnActivated;
        this.Items.ItemSelected += OnActivated;

        this.ItemDetails.Hide();

        this._hasSetup = true;
    }

    private void OnActivated(long index)
    {
        var selected = this._items[(int)index];

        this.ItemTitle.Text = selected.Name;
        this.ItemPrice.Text = $"Price: {selected.Cost} coins";
        this.ItemIcon.Texture = selected.Icon;
        this.ItemDescription.Text = selected.Description;
        this.ItemDetails.Show();
    }

    private void OnEmptyClick(Vector2 atPosition, long mouseButtonIndex)
    {
        this.ItemTitle.Text = "";
        this.ItemPrice.Text = $"Price: -1 coins";
        this.ItemIcon.Texture = null;
        this.ItemDescription.Text = "";
        this.ItemDetails.Hide();
    }

    private void Close()
    {
        this._onClose.Invoke();
        this.QueueFree();
    }

    public override void _Process(double delta)
    {
        if (!this._hasSetup) return;

        this.Items.GrabFocus();
        this.Items.GrabClickFocus();
    }
}
