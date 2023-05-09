namespace Crygotchi;

public partial class FarmingGUI : Panel
{
    [Export] public TextureRect Viewer;

    private Vector2 _focused = Vector2.Zero;
    private CursorState _cursorState;
    private bool _hasSetup = false;
    private FarmingTower _tower;

    public override void _Ready()
    {
        base._Ready();
        this._cursorState = this.GetNode<CursorState>("/root/CursorState");
        this._cursorState.SetBusy(true);
    }

    public void Setup(FarmingTower tower, SubViewport viewport)
    {
        if (this._hasSetup) return;

        this.Viewer.Texture = viewport.GetTexture();
        this._hasSetup = true;
        this._tower = tower;

        //* Set the camera initial target
        this._tower.SetCameraTarget(this._tower.GetPosition(this._focused), true);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (!this._hasSetup) return;

        if (Input.IsActionJustPressed("ui_cancel")) this.Close();

        if (Input.IsActionJustPressed("ui_right")) this._focused.X += 1;
        if (Input.IsActionJustPressed("ui_left")) this._focused.X -= 1;

        if (Input.IsActionJustPressed("ui_up")) this._focused.Y += 1;
        if (Input.IsActionJustPressed("ui_down")) this._focused.Y -= 1;

        this._focused = this._tower.ClampInput(this._focused);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        //* Update the camera target
        var targetPosition = this._tower.GetPosition(this._focused);
        this._tower.SetCameraTarget(targetPosition, false);
    }

    private void Close()
    {
        this._cursorState.SetBusy(false);
        this.QueueFree();
    }
}
