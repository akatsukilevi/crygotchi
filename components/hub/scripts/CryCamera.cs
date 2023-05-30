namespace Crygotchi;

public partial class CryCamera : Camera3D
{
    [ExportCategory("Targetting")]
    [Export] public Node3D Target;
    [Export] public Vector3 Offset;
    [Export] public float Speed = 5.0f;

    public override void _PhysicsProcess(double delta)
    {
        this.GlobalPosition = this.GlobalPosition.Lerp(
            this.Target.GlobalPosition + this.Offset,
            (float)(delta * this.Speed)
        );
    }
}
