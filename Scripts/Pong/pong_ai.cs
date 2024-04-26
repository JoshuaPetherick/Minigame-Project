using Godot;

public partial class pong_ai : CharacterBody2D
{
    public const float SPEED = 200.0f;
    public const int MAX_POSITION_CAP = 204;

    // Movement Vars
    private Node2D target;

    public void Setup(ball ball)
        => target = ball;

    public override void _PhysicsProcess(double delta)
    {
        // Setup
        float movement = 0.0f;
        float sizeY = Scale.Y / 2;
        bool up = target.Position.Y < Position.Y;
        bool down = target.Position.Y > Position.Y;

        // Checks
        if (!up & !down)
            return;

        // Apply Movement
        if (up)
            movement -= SPEED * (float)delta;

        if (down)
            movement += SPEED * (float)delta;

        // Move to New Position
        Position = new Vector2(Position.X, Position.Y + movement);

        // Position Restrictions 
        if ((Position.Y - sizeY) <= -MAX_POSITION_CAP)
            Position = new Vector2(Position.X, -MAX_POSITION_CAP - -sizeY);
        else if ((Position.Y + sizeY) >= MAX_POSITION_CAP)
            Position = new Vector2(Position.X, MAX_POSITION_CAP - sizeY);
    }
}
