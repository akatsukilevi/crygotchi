using Godot.Collections;

public partial class AvatarAnimationHelper : Node3D
{
    [ExportCategory("Animation params provider")]
    [ExportGroup("Params")]
    [Export] public Dictionary<String, bool> AnimParams;
    [Export] public AnimationTree Animator;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
