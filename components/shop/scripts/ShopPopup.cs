using System.Collections.Generic;

namespace Crygotchi;

public partial class ShopPopup : Panel
{
    [ExportCategory("References")]
    [ExportGroup("List")]
    [Export] public ItemList Items;
    [ExportGroup("Item")]
    [Export] public Control ItemDetails;
    [Export] public RichTextLabel ItemTitle;
    [Export] public RichTextLabel ItemPrice;
    [Export] public TextureRect ItemIcon;
    [Export] public RichTextLabel ItemDescription;
    [ExportGroup("Player")]
    [Export] public RichTextLabel PlayerCoins;

    private OSCController _osc;
    private SaveGame _save;

    private List<IDatabaseItem> _items = new();
    private List<IDatabaseItem> _owned = new();
    private Action<IDatabaseItem> _onPurchase;
    private Action _onClose;

    private int _currentSelected = 0;
    private bool _hasSetup = false;

    public override void _Ready()
    {
        base._Ready();

        this._save = this.GetNode<SaveManager>("/root/SaveManager").GetSave();
        this._osc = this.GetNode<OSCController>("/root/OSCController");
        this._save.OnSaveUpdated += this.OnSaveUpdated;
    }

    public void Setup(IDatabaseItem[] items, IDatabaseItem[] owned, Action<IDatabaseItem> OnPurchase, Action OnClose)
    {
        this._onPurchase = OnPurchase;
        this._onClose = OnClose;

        this._items = items.ToList();
        this._owned = owned.ToList();

        this.OnSaveUpdated();
        this.UpdateSelected();

        this._hasSetup = true;
    }

    public void UpdateItems(IDatabaseItem[] items, IDatabaseItem[] owned)
    {
        this._items = items.ToList();
        this._owned = owned.ToList();

        this.OnSaveUpdated();
        this.UpdateSelected();
    }

    private void OnSaveUpdated()
    {
        this.Items.Clear();
        foreach (var item in this._items) this.Items.AddItem(item.Name, item.Icon);
        this.PlayerCoins.Text = $"Coins: {this._save.GetCoins()}";
        this.UpdateSelected();
    }

    private void OnMove(Vector2 vector)
    {
        if (vector.Y == 0) return;

        this._currentSelected = vector.Y > 0 ? this._currentSelected + 1 : this._currentSelected - 1;
        this._currentSelected = this.ClampIndex(this._currentSelected);
        this.UpdateSelected();
    }

    private void OnBuy(IDatabaseItem item)
    {
        if (!this._save.CanAfford(item.Cost)) return; //* Cannot afford item
        this._onPurchase.Invoke(item);
    }

    private void UpdateSelected()
    {
        this._currentSelected = this.ClampIndex(this._currentSelected);
        this.Items.Select(this._currentSelected, true);
        this.UpdateDetails();
        this.UpdateKeys();
    }

    private void UpdateDetails()
    {
        if (this._items.Count < this._currentSelected) return;
        if (this._items.Count == 0) return;

        var selected = this._items[this._currentSelected];

        var isOwned = this._owned.Contains(selected);
        var priceColor = this._save.CanAfford(selected.Cost) ? "green" : "red";

        this.ItemTitle.Text = isOwned ? $"{selected.Name} ([color=green]Owned[/color])" : selected.Name;
        this.ItemPrice.Text = $"Price: [color={priceColor}]{selected.Cost} coins[/color]";
        this.ItemIcon.Texture = selected.Icon;
        this.ItemDescription.Text = selected.Description;
        this.ItemDetails.Show();
    }

    private void UpdateKeys()
    {
        var keys = new List<OSC>() {
            new() {
                Key = OSCKey.Cancel,
                Name = "Close",
                OnActivate = this.Close
            },
            new DirectionalOSC() {
                Key = OSCKey.Axis,
                Name = "Move",
                OnActivate = this.OnMove,
            }
        };

        if (this._items.Count == 0 || this._items.Count < this._currentSelected)
        {
            this._osc.RegisterOSC(keys.ToArray());
            return;
        }

        var item = this._items[this._currentSelected];
        var canBuy = this._owned.Contains(item) ? false : this._save.CanAfford(item.Cost);

        if (canBuy)
        {
            keys.Add(new()
            {
                Key = OSCKey.Primary,
                Name = "Buy",
                OnActivate = () => this.OnBuy(item)
            });
        }

        this._osc.RegisterOSC(keys.ToArray());
    }

    private int ClampIndex(int number)
    {
        int amount = this._items.Count;

        if (number >= amount)
            return 0;

        return number < 0 ? amount - 1 : number;
    }

    private void Close()
    {
        this._onClose.Invoke();
        this.QueueFree();
    }
}
