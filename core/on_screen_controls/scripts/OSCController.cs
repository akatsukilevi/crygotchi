using System.Collections.Generic;

namespace Crygotchi;

public partial class OSCController : VBoxContainer
{
    [ExportCategory("References")]
    [Export] public ItemList Controls;

    private Dictionary<OSCKey, Action<object>> _listeners = new();

    public void RegisterOSC(OSC[] keys)
    {
        this._listeners.Clear();
        this.Controls.Clear();

        foreach (var key in keys)
        {
            this.Controls.AddItem(key.Name);

            if (key is TypedOSC<object>)
            {
                this._listeners.Add(key.Key, (x) => (key as TypedOSC<object>).OnActivate(x));
                continue;
            }

            this._listeners.Add(key.Key, (_) => key.OnActivate());
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsPhysicalKeyPressed(Key.Space))
        {
            if (this._listeners.TryGetValue(OSCKey.Primary, out Action<object> handler)) handler.Invoke(null);
        }

        if (Input.IsPhysicalKeyPressed(Key.Backspace))
        {
            if (this._listeners.TryGetValue(OSCKey.Secondary, out Action<object> handler)) handler.Invoke(null);
        }

        if (Input.IsPhysicalKeyPressed(Key.Tab))
        {
            if (this._listeners.TryGetValue(OSCKey.Tertiary, out Action<object> handler)) handler.Invoke(null);
        }

        if (Input.IsPhysicalKeyPressed(Key.Q))
        {
            if (this._listeners.TryGetValue(OSCKey.ShiftLeft, out Action<object> handler)) handler.Invoke(null);
        }

        if (Input.IsPhysicalKeyPressed(Key.E))
        {
            if (this._listeners.TryGetValue(OSCKey.ShiftRight, out Action<object> handler)) handler.Invoke(null);
        }
    }
}

[Flags]
public enum OSCKey
{
    Primary = 1,
    Secondary = 2,
    Tertiary = 3,
    Axis = 4,
    ShiftLeft = 5,
    ShiftRight = 6,
}
