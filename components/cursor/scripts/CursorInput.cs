using System.Collections.Generic;

namespace Crygotchi;

public partial class CursorInput : Node
{
    private CursorState _cursorState;
    private RoomState _roomState;
    private OSCController _osc;

    public override void _Ready()
    {
        base._Ready();

        this._cursorState = GetNode<CursorState>("/root/CursorState");
        this._roomState = GetNode<RoomState>("/root/RoomState");
        this._osc = GetNode<OSCController>("/root/OSCController");

        this._roomState.OnStateChange += (bool _) => this.UpdateOSC();
        this._cursorState.OnStateChange += this.UpdateOSC;

        this.UpdateOSC();
    }

    private void UpdateOSC()
    {
        var position = this._cursorState.GetPosition();
        var input = this._roomState.GetInput(position);

        var osc = new List<OSC>()
        {
            new TypedOSC<Vector2>()
            {
                Key = OSCKey.Axis,
                OnActivate = this.OnMove,
                Name = "Move",
            }
        };

        osc.AddRange(input);

        this._osc.RegisterOSC(osc.ToArray());
    }

    private void OnMove(Vector2 delta)
    {
        var pos = this._cursorState.GetPosition();
        this._cursorState.SetPosition(pos + delta);
    }

    public override void _Input(InputEvent @event)
    {
        if (this._cursorState.IsBusy()) return;

        var pos = this._cursorState.GetPosition();

        if (Input.IsActionJustPressed("cursor_action_primary"))
            this._cursorState.CursorActionPressed(ActionType.Primary);

        if (Input.IsActionJustPressed("cursor_action_secondary"))
            this._cursorState.CursorActionPressed(ActionType.Secondary);
    }
}
