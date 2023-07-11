namespace AfterlifeAdventures;

public partial class CryCamera : Camera3D
{
    [ExportCategory("Camera Controller")]
    [ExportGroup("References")]
    [Export] public Area3D CullingArea;
    [Export] public Node3D Target;
    [ExportGroup("Settings")]
    [Export] public Vector3 Offset;
    [Export] public float Speed = 5.0f;
    [Export] public string[] CullIgnoreGroups = new string[] { };

    public override void _Ready()
    {
        base._Ready();

        this.CullingArea.BodyEntered += OnAreaCullEntered;
        this.CullingArea.BodyExited += OnAreaCullExited;
    }

    public override void _PhysicsProcess(double delta)
    {
        this.GlobalPosition = this.GlobalPosition.Lerp(
            this.Target.GlobalPosition + this.Offset,
            (float)(delta * this.Speed)
        );
    }

    private void OnAreaCullEntered(Node3D collider)
    {
        var target = collider.GetParentNode3D();
        if (target == null)
        {
            GD.PushWarning($"Could not find parent for {collider.Name}({collider})");
            return;
        }

        // TODO: Dithering effect instead of just hiding stuff would be nicer
        if (this.CanCull(collider, target)) target.Hide();
    }

    private void OnAreaCullExited(Node3D collider)
    {
        var target = collider.GetParentNode3D();
        if (target == null) GD.PushWarning($"Could not find parent for {collider.Name}({collider})");

        if (this.CanCull(collider, target)) target.Show();
    }

    private bool CanCull(Node3D collider, Node3D target)
    {
        foreach (var group in this.CullIgnoreGroups)
        {
            if (collider.IsInGroup(group)) return false; //* Fail if the collider is on the group
            if (target.IsInGroup(group)) return false; //* Fail if the target is on the group
        }

        return true; //* Neither are, can continue
    }
}
