using Godot;
using System;

public partial class pong_ai : CharacterBody2D
{
    public const float Speed = 200.0f;

    // Movement Vars
    private Node2D target;

    public void Setup(ball ball)
        => target = ball;

    public override void _PhysicsProcess(double delta)
    {
        // Setup
        float movement = 0.0f;
        bool up = target.Position.Y < Position.Y;
        bool down = target.Position.Y > Position.Y;

        // Checks
        if (!up & !down)
            return;

        // Apply Movement
        if (up)
            movement -= Speed * (float)delta;

        if (down)
            movement += Speed * (float)delta;

        // Move to New Position
        Position = new Vector2(Position.X, Position.Y + movement);

        // Hardcoded Restrictions (Fix later)
        if (Position.Y <= -117.5f)
            Position = new Vector2(Position.X, -117.5f);
        else if (Position.Y >= 118.5f)
            Position = new Vector2(Position.X, 118.5f);
    }
}
