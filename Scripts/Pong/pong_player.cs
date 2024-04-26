using Godot;

public partial class pong_player : CharacterBody2D
{
	public const float SPEED = 200.0f;
    public const int MAX_POSITION_CAP = 204;
    
    // Movement Vars
    private bool _goingUp = false;
	private Vector2 _previousPosition;

	public override void _PhysicsProcess(double delta)
	{
        // Multiplayer Check
        if (Multiplayer.MultiplayerPeer != null)
        {
            // Checks
            if (!IsMultiplayerAuthority())
                return;

            // Move Ball
            CalculateMovement((float)delta);
        }
        else
            CalculateMovement((float)delta);
    }

    #region Functions

	private void CalculateMovement(float delta)
	{
        // Setup
        bool up = false;
        bool down = false;
        float movement = 0.0f;
        float sizeY = Scale.Y / 2;

        // Get Inputs based on Name
        switch (Name)
        {
            case "Player 1":
                up = Input.IsActionPressed("up_player_1");
                down = Input.IsActionPressed("down_player_1");
                break;

            case "Player 2":
                up = Input.IsActionPressed("up_player_2");
                down = Input.IsActionPressed("down_player_2");
                break;

            default:
                up = Input.IsActionPressed("up");
                down = Input.IsActionPressed("down");
                break;
        }

        // Checks
        if (!up & !down)
            return;

        // Apply Movement
        if (up)
            movement -= SPEED * delta;

        if (down)
            movement += SPEED * delta;

        // Store Previous Position
        _goingUp = up && !down ? true : false;
        _previousPosition = Position;

        // Move to New Position
        Position = new Vector2(Position.X, Position.Y + movement);

        // Position Restrictions 
        if ((Position.Y - sizeY) <= -MAX_POSITION_CAP)
            Position = new Vector2(Position.X, -MAX_POSITION_CAP - -sizeY);
        else if ((Position.Y + sizeY) >= MAX_POSITION_CAP)
            Position = new Vector2(Position.X, MAX_POSITION_CAP - sizeY);
    }

    public void CollidedWithWall()
	{
		// Reset Position
		Position = _previousPosition;
    }

    #endregion
}
