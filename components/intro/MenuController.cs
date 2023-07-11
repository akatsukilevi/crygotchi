namespace AfterlifeAdventures;

public partial class MenuController : Control
{
    [ExportCategory("References")]
    [Export] private AnimationPlayer Animator;

    private bool _canCheckInput = false;
    private AppState _appState;

    public override void _Ready()
    {
        this._appState = this.GetNode<AppState>("/root/AppState");
    }

    public override void _Input(InputEvent @event)
    {
        if (!this._canCheckInput) return;
        if (!this._appState.IsMenuOpen()) return;

        this.Animator.Play("fade_out");
        this._canCheckInput = false;
    }

    private void EnableInputWait()
    {
        this._canCheckInput = true;
    }

    private void OnMenuFadedOut()
    {
        //* Then can close here
        this._appState.CloseMenu();
    }
}
