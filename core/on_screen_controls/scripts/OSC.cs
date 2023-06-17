namespace Crygotchi;

public class OSC
{
    public string Name;
    public OSCKey Key;
    public Action OnActivate;
}

public class TypedOSC<T> : OSC
{
    public new Action<T> OnActivate;
}
