using System.Collections.Generic;

namespace Crygotchi;

public partial class CursorInput : Node
{
    private CursorState _cursorState;
    private RoomState _roomState;
    private OSCController _osc;
    private AppState _appState;

    public override void _Ready()
    {
        base._Ready();

        this._cursorState = GetNode<CursorState>("/root/CursorState");
        this._roomState = GetNode<RoomState>("/root/RoomState");
        this._osc = GetNode<OSCController>("/root/OSCController");
        this._appState = GetNode<AppState>("/root/AppState");

        this._roomState.OnStateChange += (bool _) => this.UpdateOSC();
        this._cursorState.OnStateChange += this.UpdateOSC;
        this._appState.OnMainMenuClose += this.UpdateOSC;

        this.UpdateOSC();
    }

    private void UpdateOSC()
    {
        if (this._appState.IsMenuOpen()) return;
        if (this._cursorState.IsBusy()) return;

        var position = this._cursorState.GetPosition();
        var input = this._roomState.GetInput(position);

        var osc = new List<OSC>()
        {
            new DirectionalOSC()
            {
                Key = OSCKey.Axis,
                OnActivate = (Vector2 delta) => this.OnMove(delta),
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
}
