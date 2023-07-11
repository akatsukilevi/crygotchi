namespace AfterlifeAdventures;

public partial class CryState : Node
{
    private bool _isBusy = false;

    public event Action OnStateChange;


    public override void _Ready()
    {
        base._Ready();
    }

    #region "General"
    public bool IsBusy()
    {
        return this._isBusy;
    }

    public void AcquireBusyState()
    {
        if (this._isBusy) throw new Exception("Cry is already busy, cannot acquire another busy state");

        this._isBusy = true;
        this.OnStateChange?.Invoke();
    }

    public void ReleaseBusyState()
    {
        if (!this._isBusy) throw new Exception("Cry is not busy, cannot release non-existent busy state");

        this._isBusy = false;
        this.OnStateChange?.Invoke();
    }
    #endregion
}
