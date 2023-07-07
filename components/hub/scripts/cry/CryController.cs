namespace Crygotchi;

// TODO: Better Cry Input handling
//! Cry controls shouldn't be hardcoded like it is now

public partial class CryController : CharacterBody3D
{
    [ExportCategory("Character Controller")]
    [ExportGroup("References")]
    [Export] public Node3D Camera;
    [Export] public AvatarAnimationHelper AAHelper;

    [ExportGroup("Settings")]

    //private AnimationTree _animator;

    private Vector3 _inputDirection = Vector3.Zero;
    private Vector3 _moveDirection = Vector3.Zero;
    private bool _isRunning = false;

    private float _smoothAngle;

    private Vector3 _gravity;
    private Vector3 animatorVelocity;

    public override void _Ready()
    {
        //this._state = this.GetNode<CryState>("/root/CryState");
        AAHelper.Animator.Active = true;
        _gravity = ((float)ProjectSettings.GetSetting("physics/3d/default_gravity")) * ((Vector3)ProjectSettings.GetSetting("physics/3d/default_gravity_vector"));
    }

    public override void _Process(double delta)
    {
        _inputDirection = GetInputDirection();
        _isRunning = Input.IsKeyPressed(Key.Shift);

        if (_inputDirection.Normalized().Length() > 0.01f)
        {
            float targetAngle = Mathf.Atan2(_inputDirection.X, _inputDirection.Z) * MathUtils.Rad2Deg + Camera.GlobalRotation.Y * MathUtils.Rad2Deg;
            float smoothTarget = MathUtils.SmoothDampAngle(GlobalRotation.Y * MathUtils.Rad2Deg, targetAngle, ref _smoothAngle, 0.1f, Mathf.Inf, delta);
            GlobalRotation = new Vector3(0.0f, smoothTarget * MathUtils.Deg2Rad, 0.0f);

            //Set proper flag
            AAHelper.AnimParams["Walking"] = true;
        }
        else
        {
            AAHelper.AnimParams["Walking"] = false;
        }
        AAHelper.AnimParams["Sprinting"] = _isRunning;
        animatorVelocity = (Transform.Basis.GetRotationQuaternion().Normalized() * AAHelper.Animator.GetRootMotionPosition()) / (float)delta;
    }

    public override void _PhysicsProcess(double delta)
    {

        animatorVelocity += _gravity * (float)delta + new Vector3(0.0f, Velocity.Y, 0.0f);
        Velocity = animatorVelocity;
        MoveAndSlide();
    }

    private Vector3 GetInputDirection()
    {
        return new Vector3(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            0.0f,
            Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
        );
    }

    //private Vector3 GetVelocity(Vector3 direction, float delta)
    //{
    //    Vector3 velocity = direction * delta * this.MoveSpeed;

    //    //* Clamp the velocity down
    //    if (velocity.Length() > this.MaxSpeed) velocity = velocity.Normalized() * this.MaxSpeed;

    //    if (this._isRunning) velocity *= 5f;

    //    //* Add gravity force and return it
    //    velocity.Y = velocity.Y + this.Gravity * delta;
    //    return velocity;
    //}
}
