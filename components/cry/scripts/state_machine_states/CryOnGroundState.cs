namespace AfterlifeAdventures.CryRoomStates;

public class CryOnGroundState : CryAvatarRoomState
{
    protected static Vector3 animatorVelocity;
    protected static Vector3 gravity;
    protected static Vector3 targetPosition = Vector3.Zero;
    protected static float smoothAngle;
    public CryOnGroundState(CryAvatarRoomStateMachine stateMachine) : base(stateMachine)
    {
        gravity = ((float)ProjectSettings.GetSetting("physics/3d/default_gravity")) * ((Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector"));
    }

    public override void Process(double delta)
    {
        if (stateMachine.Avatar.Position.DistanceTo(targetPosition) > 0.1f)
        {
            float targetAngle = Transform3D.Identity.LookingAt((stateMachine.Avatar.Position - targetPosition).Normalized(), Vector3.Up).Basis.GetEuler().Y * MathUtils.Rad2Deg;
            float smoothTarget = MathUtils.SmoothDampAngle(stateMachine.Avatar.GlobalRotation.Y * MathUtils.Rad2Deg, targetAngle, ref smoothAngle, 0.1f, Mathf.Inf, delta);
            stateMachine.Avatar.GlobalRotation = new Vector3(0.0f, smoothTarget * MathUtils.Deg2Rad, 0.0f);
        }
        stateMachine.Avatar.GlobalPosition += stateMachine.Avatar.Transform.Basis.GetRotationQuaternion().Normalized() * stateMachine.Avatar.AAHelper.Animator.GetRootMotionPosition();
    }

    public override void PhysicsProcess(double delta)
    {
        //No Collision and gravity yet
        //animatorVelocity += gravity * (float)delta + new Vector3(0.0f, stateMachine.Avatar.Velocity.Y, 0.0f);
        //stateMachine.Avatar.Velocity = animatorVelocity;
        //stateMachine.Avatar.MoveAndSlide();
    }
}

