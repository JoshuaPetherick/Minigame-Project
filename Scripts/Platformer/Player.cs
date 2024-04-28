using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = 400.0f;

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
            RotationDegrees = 90;
        }
        if (Input.IsActionJustPressed("up_player_2"))
        {
            CurrentGravity = Gravity.North;
            UpDirection = new Vector2(0, 1);
            RotationDegrees = 180;
        }
        if (Input.IsActionJustPressed("right_player_2"))
        {
            CurrentGravity = Gravity.West;
            UpDirection = new Vector2(-1, 0);
            RotationDegrees = 270;
        }
        if (Input.IsActionJustPressed("down_player_2"))
		{
            CurrentGravity = Gravity.South;
            UpDirection = new Vector2(0, -1);
            RotationDegrees = 0;
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
        if (!IsOnFloor())
            return velocity;

        // Setup
        Vector2 result = velocity;
        Vector2 direction = Input.GetVector("left_player_1", "right_player_1", "up_player_1", "down_player_1");

        // Move based on Gravity
        switch (CurrentGravity)
        {
            case Gravity.North:
            case Gravity.South:
                if (direction != Vector2.Zero)
                    result.X = direction.X * Speed;
                else
                    result.X = Mathf.MoveToward(Velocity.X, 0, Speed);
                break;

            case Gravity.East:
                if (direction != Vector2.Zero)
                    result.Y = direction.X * Speed;
                else
                    result.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
                break;

            case Gravity.West:
                if (direction != Vector2.Zero)
                    result.Y = -direction.X * Speed;
                else
                    result.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
                break;
        }

        // Result
        return result;
    }

    #endregion
}
