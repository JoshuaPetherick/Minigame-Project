using Godot;
using System;

public partial class player : CharacterBody2D
{
	public const float Speed = 200.0f;

	// Movement Vars
	private bool _goingUp = false;
	private Vector2 _previousPosition;

	// Restriction Vars
	private bool _restrictUp = false;
	private bool _restrictDown = false; 

	public override void _PhysicsProcess(double delta)
	{
		// Setup
		float movement = 0.0f;
		bool up = Input.IsActionPressed("ui_up");
		bool down = Input.IsActionPressed("ui_down");

		// Checks
		if (!up & !down)
			return;

        // Apply Movement
		if (up)
			movement -= Speed * (float)delta;

		if (down)
            movement += Speed * (float)delta;

		// Restriction Checks
		if (_restrictUp && up)
			return;

        if (_restrictDown && down)
            return;

		// Release Restriction
		if (_restrictUp && !up)
			_restrictUp = false;

		if (_restrictDown && !down)
			_restrictDown = false;

        // Store Previous Position
        _goingUp = up && !down ? true : false;
        _previousPosition = Position;

		// Move to New Position
        Position = new Vector2(Position.X, Position.Y + movement);
	}

	public void CollidedWithWall()
	{
		// Reset Position
		Position = _previousPosition;

		// Restrict Further Movement
		_restrictUp = _goingUp;
		_restrictDown = !_goingUp;
    }
}
