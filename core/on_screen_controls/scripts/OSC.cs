namespace Crygotchi;

public class OSC
{
    public string Name;
    public OSCKey Key;
    public Action OnActivate;
}

public class DirectionalOSC : OSC
{
    public new Action<Vector2> OnActivate;
}
