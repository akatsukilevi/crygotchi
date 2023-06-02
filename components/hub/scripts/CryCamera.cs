using System.Collections.Generic;
using System.Linq;

namespace Crygotchi;

public partial class CryCamera : Camera3D
{
    [ExportCategory("Camera Controller")]
    [ExportGroup("References")]
    [Export] public Area3D CullingArea;
    [Export] public Node3D Target;
    [ExportGroup("Settings")]
    [Export] public Vector3 Offset;
    [Export] public float Speed = 5.0f;
    [Export] public string[] CullingWhitelist = new string[] { };

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
        if (collider.Name.ToString().Contains("OOB")) return; //* Don't touch the OOB invisible walls

        var target = collider.GetParentNode3D();
        if (target == null)
        {
            GD.PushWarning($"Could not find parent for {collider.Name}({collider})");
            return;
        }

        // TODO: Dithering effect instead of just hiding stuff would be nicer
        if (this.IsObjectCullable(target.Name)) target.Hide();
    }

    private void OnAreaCullExited(Node3D collider)
    {
        if (collider.Name.ToString().Contains("OOB")) return; //* Don't touch the OOB invisible walls

        var target = collider.GetParentNode3D();
        if (target == null) GD.PushWarning($"Could not find parent for {collider.Name}({collider})");

        if (this.IsObjectCullable(target.Name)) target.Show();
    }


    //! There should be a better way to do this
    //! But Godot doesn't like when I export a List<T>
    //! And keep calling `ToList` every time something needs to be culled is Not Good(tm)
    //! Also need to match partial names, soooooo
    //! Until me and my last two braincells can think on a better solution
    //! That's what I'll be doing
    private bool IsObjectCullable(string targetName)
    {
        foreach (string name in this.CullingWhitelist)
        {
            if (targetName.ToLower() == name.ToLower()) return false; //* If the name is the same
            if (targetName.ToLower().Contains(name.ToLower())) return false; //* If the name includes it
        }

        return true;
    }
}
