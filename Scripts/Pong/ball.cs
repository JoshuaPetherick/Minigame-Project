using Godot;
using System;

public partial class ball : Area2D
{
	public const float START_SPEED = 100.0f;
    public const float SPEED_INCREASE = 25.0f;

    // Movement Vars
	private bool _up = false;
	private bool _left = false;
    private float _currentSpeed = START_SPEED;

    public override void _Ready()
    {
        AreaEntered += Ball_AreaEntered;
        BodyEntered += Ball_BodyEntered;
    }

    public override void _PhysicsProcess(double delta)
	{
		// Setup
		float movementX = 0.0f;
        float movementY = 0.0f;

        // Apply Movement
        if (_left)
            movementX -= _currentSpeed * (float)delta;
        else
            movementX += _currentSpeed * (float)delta;

        if (_up)
            movementY -= _currentSpeed * (float)delta;
        else
            movementY += _currentSpeed * (float)delta;

        // Move to New Position
        Position = new Vector2(Position.X + movementX, Position.Y + movementY);
    }

    #region Events

    private void Ball_AreaEntered(Area2D area)
    {
        // Check - Change Vertical/Horizontal
        if (area is player_wall)
            _left = !_left;
        else
            _up = !_up;
    }

    private void Ball_BodyEntered(Node2D body)
    {
        // Change Horizontal
        _left = !_left;

        // Increase Speed
        if (body is player)
            _currentSpeed += SPEED_INCREASE;
    }

    #endregion

    #region Public Functions

    public void Reset()
    {
        // Reset Position
        Position = Vector2.Zero;

        // Reset Movement Vars
        _up = !_up;
        _left = !_left;
        _currentSpeed = START_SPEED;
    }

    #endregion
}
