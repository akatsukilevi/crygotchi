using Godot.Collections;

namespace Crygotchi;

public partial class SaveGame : Resource
{
    private Room _roomData = new();
    private Cursor _cursorData = new();
    private int _coins = 0;

    private CursorState _cursorState;
    private RoomState _roomState;

    public void Initialize(Node manager)
    {
        this._cursorState = manager.GetNode<CursorState>("/root/CursorState");
        this._roomState = manager.GetNode<RoomState>("/root/RoomState");
    }

    public void UpdateSave()
    {
        this._roomData.Tiles = this._roomState.GetTiles();
        this._roomData.Mode = this._roomState.GetMode();

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
}
