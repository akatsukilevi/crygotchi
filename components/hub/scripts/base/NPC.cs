using System.Threading.Tasks;

namespace Crygotchi;

public abstract partial class NPC : Node3D
{
    private CryState _playerState;
    private Area3D _triggerZone;

    private bool _isInteractable = false;

    public override void _Ready()
    {
        this._triggerZone = this.GetNode<Area3D>("./TriggerZone");
        this._playerState = this.GetNode<CryState>("/root/CryState");

        this._triggerZone.AreaEntered += OnAreaCullEntered;
        this._triggerZone.AreaExited += OnAreaCullExited;
    }

    public override void _Process(double delta)
    {
        if (!this._isInteractable) return;
        if (!Input.IsActionJustPressed("cursor_action_primary")) return;

        GD.Print("[ NPC ] Requested interaction");
        this.Interact();
    }

    private void OnAreaCullEntered(Area3D collider)
    {
        if (!collider.IsInGroup("Hub_Player")) return;

        GD.Print("[ NPC ] Detected player near!");
        this._isInteractable = true;
    }

    private void OnAreaCullExited(Area3D collider)
    {
        if (!collider.IsInGroup("Hub_Player")) return;

        GD.Print("[ NPC ] Detected player far!");
        this._isInteractable = false;
    }

    private void Interact()
    {
        GD.Print("[ NPC ] Acquiring lock");
        this._playerState.AcquireBusyState();

        GD.Print("[ NPC ] Init interaction");
        this.StartInteraction();
    }

    protected abstract Task StartInteraction();

    protected virtual Task FinishInteraction()
    {
        this._playerState.ReleaseBusyState();
        return Task.CompletedTask;
    }
}
