namespace AfterlifeAdventures;

public abstract partial class NPC : Node3D
{
    protected CryState _playerState;
    private Area3D _triggerZone;
    private OSCController _osc;

    private bool _isInteractable = false;
    private bool _isInteracting = false;

    public override void _Ready()
    {
        this._triggerZone = this.GetNode<Area3D>("./TriggerZone");
        this._playerState = this.GetNode<CryState>("/root/CryState");
        this._osc = this.GetNode<OSCController>("/root/OSCController");

        this._triggerZone.AreaEntered += OnAreaCullEntered;
        this._triggerZone.AreaExited += OnAreaCullExited;
    }

    private void UpdateInput()
    {
        if (this._isInteracting) return;
        if (!this._isInteractable)
        {
            this._osc.ClearOSC();
            return;
        }

        this._osc.RegisterOSC(new OSC[] {
            new() {
                Key = OSCKey.Primary,
                Name = "Interact",
                OnActivate = this.Interact
            }
        });
    }

    private void OnAreaCullEntered(Area3D collider)
    {
        if (!collider.IsInGroup("Hub_Player")) return;

        GD.Print("[ NPC ] Detected player near!");
        this._isInteractable = true;
        this.UpdateInput();
    }

    private void OnAreaCullExited(Area3D collider)
    {
        if (!collider.IsInGroup("Hub_Player")) return;

        GD.Print("[ NPC ] Detected player far!");
        this._isInteractable = false;
        this.UpdateInput();
    }

    private void Interact()
    {
        GD.Print("[ NPC ] Acquiring lock");
        this._playerState.AcquireBusyState();
        this._isInteracting = true;
        this.UpdateInput();

        GD.Print("[ NPC ] Init interaction");
        this.StartInteraction();
    }

    protected abstract void StartInteraction();

    protected virtual void FinishInteraction()
    {
        this._playerState.ReleaseBusyState();
        this._isInteracting = false;
        this.UpdateInput();
    }
}
