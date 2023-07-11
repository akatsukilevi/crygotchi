using Godot.Collections;

namespace AfterlifeAdventures;

public partial class SaveGame : Resource
{
    public event Action OnSaveUpdated;

    private Room _roomData = new();
    private Cursor _cursorData = new();
    private int _coins = 0;

    private TilesDatabase _tilesDatabase;
    private ItemsDatabase _itemsDatabase;

    private CursorState _cursorState;
    private RoomState _roomState;

    public void Initialize(Node manager)
    {
        this._tilesDatabase = manager.GetNode<TilesDatabase>("/root/TilesDatabase");
        this._itemsDatabase = manager.GetNode<ItemsDatabase>("/root/ItemsDatabase");

        this._cursorState = manager.GetNode<CursorState>("/root/CursorState");
        this._roomState = manager.GetNode<RoomState>("/root/RoomState");
    }

    public Cursor GetCursorSaveState()
    {
        return this._cursorData;
    }

    public Room GetRoomSaveState()
    {
        return this._roomData;
    }

    public void UpdateSave()
    {
        this._roomData.Tiles = this._roomState.GetTiles();

        this._cursorData.Position = this._cursorState.GetPosition();
        this._cursorData.HeldItemID = this._cursorState.PeekItem()?.GetId();
    }

    public Dictionary<string, Variant> SerializeSave()
    {
        return new Dictionary<string, Variant>()
        {
            { "Room", this._roomData.Serialize() },
            { "Cursor", this._cursorData.Serialize() },
            { "Coins", this._coins },
        };
    }

    public void DeserializeSave(Dictionary<string, Variant> data)
    {
        this._roomData = new();
        this._cursorData = new();

        this._coins = (int)data["Coins"];
        this._roomData.Deserialize((Dictionary<string, Variant>)data["Room"], this._tilesDatabase, this._itemsDatabase);
        this._cursorData.Deserialize((Dictionary<string, Variant>)data["Cursor"]);

        this.OnSaveUpdated?.Invoke();
    }

    public int GetCoins()
    {
        return this._coins;
    }

    public bool CanAfford(int cost)
    {
        return this._coins >= Math.Max(cost, 0);
    }

    public void SpendCoins(int amount)
    {
        if (!this.CanAfford(amount)) return; //* Cannot afford it
        this._coins = Math.Max(this._coins - Math.Max(amount, 0), 0); //* Spend the coins but clamp to 0
        this.OnSaveUpdated?.Invoke();
    }
}
