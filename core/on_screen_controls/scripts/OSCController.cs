using System.Collections.Generic;

namespace Crygotchi;

public partial class OSCController : VBoxContainer
{
    [ExportCategory("References")]
    [Export] public ItemList Controls;
    [Export] public Texture2D DpadIcon;

    private Dictionary<string, Action<object>> _listeners = new();
    private bool _isSetup = false;
    private Node _controllerIcons;

    public override void _Ready()
    {
        base._Ready();

        this._controllerIcons = this.GetNode("/root/ControllerIcons");
    }

    public void RegisterOSC(OSC[] keys)
    {
        var newKeys = new Dictionary<string, Action<object>>();
        this._isSetup = false;
        this.Controls.Clear();

        foreach (var input in keys)
        {
            var input_name = this.KeyToInput(input.Key);
            var input_icon = this.GetKeyIcon(input.Key);

            if (newKeys.ContainsKey(input_name))
            {
                GD.PrintErr($"Attempted to register duplicated input ${input_name}: {input.Name}");
                continue;
            }

            this.Controls.AddItem(input.Name, input_icon);

            if (input is DirectionalOSC)
            {
                newKeys.Add(input_name, (x) => (input as DirectionalOSC).OnActivate?.Invoke((Vector2)x));
                continue;
            }

            newKeys.Add(input_name, (_) => input.OnActivate?.Invoke());
        }

        this._listeners = newKeys;
        this._isSetup = true;
    }

    public void ClearOSC()
    {
        this._isSetup = false;
        this.Controls.Clear();
        this._listeners.Clear();
        this._isSetup = true;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (!this._isSetup) return;

        //* Default keys
        var keys = this._listeners.Keys;
        foreach (var input in keys)
        {
            if (input == "input_axis" || !@event.IsActionReleased(input)) continue;
            this._listeners[input].Invoke(null);
        }

        //* Grab the DPAD keys
        Vector2 dpad = this.CheckDPAD(@event);
        if (dpad != Vector2.Zero)
        {
            if (!this._listeners.TryGetValue("input_axis", out Action<object> handle)) return;
            handle.Invoke(dpad);
        }
    }

    private Texture2D GetKeyIcon(OSCKey key)
    {
        if (key == OSCKey.Axis) return this.DpadIcon;

        return (Texture2D)this._controllerIcons.Call("parse_path", this.KeyToInput(key));
    }

    private Vector2 CheckDPAD(InputEvent @event)
    {
        Vector2 input = Vector2.Zero;

        if (@event.IsActionPressed("input_axis_up")) input += new Vector2(0, 1);
        if (@event.IsActionPressed("input_axis_down")) input += new Vector2(0, -1);
        if (@event.IsActionPressed("input_axis_left")) input += new Vector2(1, 0);
        if (@event.IsActionPressed("input_axis_right")) input += new Vector2(-1, 0);

        return input;
    }

    private string KeyToInput(OSCKey key)
    {
        return key switch
        {
            OSCKey.Primary => "input_primary",
            OSCKey.Secondary => "input_secondary",
            OSCKey.Tertiary => "input_tertiary",
            OSCKey.Cancel => "ui_cancel",
            OSCKey.Axis => "input_axis",
            OSCKey.ShiftLeft => "input_shift_left",
            OSCKey.ShiftRight => "input_shift_right",
            _ => "input_tertiary"
        };
    }
}

[Flags]
public enum OSCKey
{
    Primary = 1,
    Secondary = 2,
    Cancel = 3,
    Tertiary = 4,
    Axis = 5,
    ShiftLeft = 6,
    ShiftRight = 7,
}
