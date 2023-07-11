namespace AfterlifeAdventures;

public partial class AppState : Node
{
    public event Action OnMainMenuClose;

    private bool _isMainMenuOpen = true;

    public void CloseMenu()
    {
        this._isMainMenuOpen = false;
        this.OnMainMenuClose?.Invoke();
    }

    public bool IsMenuOpen()
    {
        return this._isMainMenuOpen;
    }
}
