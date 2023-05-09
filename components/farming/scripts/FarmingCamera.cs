namespace Crygotchi;

public partial class FarmingCamera : Camera3D
{
    [Export] public float Speed = 1f;
    [Export] public Vector2 Offset;

    private Node3D _target;

    public void SetTarget(Node3D target)
    {
        this._target = target;

        var targetPos = this.GetTargetPos();
        this.GlobalPosition = targetPos;
        this.LookAt(target.GlobalPosition);
    }

    public void LerpTarget(Node3D target)
    {
        this._target = target;
    }

    public override void _PhysicsProcess(double delta)
    {
        var targetPos = this.GetTargetPos();

        if (this.GlobalPosition == targetPos) return;

        this.GlobalPosition = this.GlobalPosition.Lerp(
            targetPos,
            (float)(delta * this.Speed)
        );

        this.LookAt(this._target.GlobalPosition);
    }

    private Vector3 GetTargetPos()
    {
        if (this._target == null) return this.GlobalPosition;

        var targetPos = this._target.GlobalPosition;

        targetPos += this._target.GlobalTransform.Basis.Z * Offset.X;
        targetPos += this._target.GlobalTransform.Basis.Y * Offset.Y;

        return targetPos;
    }
}
