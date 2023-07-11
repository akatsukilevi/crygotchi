namespace AfterlifeAdventures;

public partial class BobDoe : NPC
{
    private ShopService _shop;
    private TilesDatabase _tiles;
    private SaveGame _save;

    private ShopPopup _currentShop;

    public override void _Ready()
    {
        base._Ready();

        this._save = this.GetNode<SaveManager>("/root/SaveManager").GetSave();
        this._tiles = this.GetNode<TilesDatabase>("/root/TilesDatabase");
        this._shop = this.GetNode<ShopService>("/root/ShopService");
    }

    protected override void StartInteraction()
    {
        var ownedList = this._tiles.GetShopOwnedTiles().ToArray();
        var buyList = this._tiles.GetShopTiles().ToArray();

        this._currentShop = this._shop.OpenShop(buyList, ownedList, this.OnShopPurchase, this.OnShopClose);
    }

    private void OnShopPurchase(IDatabaseItem item)
    {
        if (!this._save.CanAfford(item.Cost)) return; //* Cannot afford item

        //* Bought something, update the tiles
        this._save.GetRoomSaveState().UnlockTile((RoomTile)item);

        var ownedList = this._tiles.GetShopOwnedTiles().ToArray();
        var buyList = this._tiles.GetShopTiles().ToArray();
        this._currentShop.UpdateItems(buyList, ownedList);

        this._save.SpendCoins(item.Cost);
    }

    private void OnShopClose()
    {
        this._currentShop = null;
        this.FinishInteraction();
    }
}
