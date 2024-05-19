using Godot;

public partial class Player : CharacterBody2D
{
	public const float Speed = 125.0f;
	public const float JumpVelocity = 300.0f;

    [Export]
    private AnimatedSprite2D _animatedSprite;

	public enum Gravity
	{
		North,
		South,
		East,
		West
	};
	public Gravity CurrentGravity = Gravity.South;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private float _targetRotation = 0.0f;

	public override void _PhysicsProcess(double delta)
	{
		// Setup
		Vector2 velocity = Velocity;

        // Add the gravity.
        velocity = HandleGravity(velocity, (float)delta);

        // Handle Jump.
		velocity = HandleJumping(velocity);

        // Get the input direction and handle the movement/deceleration.
        velocity = HandleMovement(velocity, (float)delta);

        // Handle Rotation
        HandleRotation();

        // Apply Velocity Changes
        Velocity = velocity;
		MoveAndSlide();
	}

    #region Functions

	private Vector2 HandleGravity(Vector2 velocity, float delta)
	{
        // Setup
        Vector2 result = velocity;

		// Input Check
        if (Input.IsActionJustPressed("left_player_2"))
        {
            CurrentGravity = Gravity.East;
            UpDirection = new Vector2(1, 0);
            _targetRotation = 90.0f;
        }
        if (Input.IsActionJustPressed("up_player_2"))
        {
            CurrentGravity = Gravity.North;
            UpDirection = new Vector2(0, 1);
            _targetRotation = 180.0f;
        }
        if (Input.IsActionJustPressed("right_player_2"))
        {
            CurrentGravity = Gravity.West;
            UpDirection = new Vector2(-1, 0);
            _targetRotation = 270.0f;
            RotationDegrees = _targetRotation - RotationDegrees > 180 ? 360 : RotationDegrees;
        }
        if (Input.IsActionJustPressed("down_player_2"))
		{
            CurrentGravity = Gravity.South;
            UpDirection = new Vector2(0, -1);
            RotationDegrees = (RotationDegrees < 0 ? RotationDegrees + 360.0f : RotationDegrees);
            _targetRotation = RotationDegrees > 180 ? 360.0f : 0.0f;
        }

		// Handle Gravity
		if (!IsOnFloor())
		{
			switch (CurrentGravity)
			{
				case Gravity.North:
                    result.Y -= gravity * delta;
					break;
                case Gravity.East:
                    result.X -= gravity * delta;
                    break;
                case Gravity.West:
                    result.X += gravity * delta;
                    break;
                case Gravity.South:
                    result.Y += gravity * delta;
                    break;
			}
		}

        // Result
        return result;
    }

	private Vector2 HandleJumping(Vector2 velocity)
	{
		// Checks
		if (!IsOnFloor())
			return velocity; 

        if (!Input.IsActionJustPressed("ui_accept"))
			return velocity;

        // Setup
        Vector2 result = velocity;

		// Jump
        switch (CurrentGravity)
        {
            case Gravity.North:
                result.Y = JumpVelocity;
                break;
            case Gravity.East:
                result.X = JumpVelocity;
                break;
            case Gravity.West:
                result.X = -JumpVelocity;
                break;
            case Gravity.South:
                result.Y = -JumpVelocity;
                break;
        }

        // Result
        return result;
    }

	private Vector2 HandleMovement(Vector2 velocity, float delta)
	{
        // Checks
        //if (!IsOnFloor())
        //    return velocity;

        // Setup
        Vector2 result = velocity;
        Vector2 direction = Input.GetVector("left_player_1", "right_player_1", "up_player_1", "down_player_1");

        // Move based on Gravity
        switch (CurrentGravity)
        {
            case Gravity.North:
            case Gravity.South:
                if (direction != Vector2.Zero)
                {
                    result.X = direction.X * Speed;
                    _animatedSprite.FlipH = CurrentGravity == Gravity.North ? direction.X > 0 : direction.X < 0;
                }
                else
                    result.X = Mathf.MoveToward(Velocity.X, 0, Speed);
                break;

            case Gravity.East:
            case Gravity.West:
                if (direction != Vector2.Zero)
                {
                    result.Y = direction.Y * Speed;
                    _animatedSprite.FlipH = CurrentGravity == Gravity.West ? direction.Y > 0 : direction.Y < 0;
                }
                else
                    result.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
                break;
        }

        // Result
        return result;
    }

    private void HandleRotation()
    {
        // Negative Handle
        if (RotationDegrees < 0)
            RotationDegrees += 360.0f;

        // Check
        if (RotationDegrees == _targetRotation)
            return;

        // Apply Rotation
        RotationDegrees = Mathf.Abs(Mathf.MoveToward(RotationDegrees, _targetRotation, 7.5f));

        // 360 Degree Check
        if (RotationDegrees == 360.0f && _targetRotation == 360.0f)
        {
            RotationDegrees = 0;
            _targetRotation = 0;
        }
    }

    #endregion
}
