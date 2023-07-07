using Godot;
using System;

public partial class CharacterMover : Node3D
{
    private AnimationTree _animTree;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _animTree = this.GetNode<AnimationTree>("AnimationTree");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        this.GlobalPosition += _animTree.GetRootMotionPosition();
    }
}
