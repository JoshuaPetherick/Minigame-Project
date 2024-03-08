using Godot;
using System;

public partial class ball : Area2D
{
	public const float Speed = 100.0f;

    // Movement Vars
	private bool _up = false;
	private bool _left = false;
    private float _currentSpeed = Speed;

    public override void _Ready()
    {
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

    private void Ball_BodyEntered(Node2D body)
    {
        _up = !_up;
        _left = !_left;

        GD.Print("Collision!");
    }
}
