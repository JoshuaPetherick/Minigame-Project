using Godot;

public partial class ball : Area2D
{
	public const float START_SPEED = 200.0f;
    public const float SPEED_INCREASE = 25.0f;

    // Movement Vars
	private bool _up = false;
	private bool _left = false;
    private Node _recollisionCheck = null;
    private float _currentSpeed = START_SPEED;

    public override void _Ready()
    {
        // Multiplayer Check
        if (Multiplayer.MultiplayerPeer != null)
        {
            // Checks
            if (!Multiplayer.IsServer())
                return;
        }

        // Setup Collision Events
        AreaEntered += Ball_AreaEntered;
        BodyEntered += Ball_BodyEntered;
    }

    public override void _PhysicsProcess(double delta)
	{
        // Multiplayer Check
        if (Multiplayer.MultiplayerPeer != null)
        {
            // Checks
            if (!Multiplayer.IsServer())
                return;
        }

        // Move Ball
        CalculateMovement((float)delta);
    }

    #region Events

    private void Ball_AreaEntered(Area2D area)
    {
        // Multiplayer Check
        if (Multiplayer.MultiplayerPeer != null)
        {
            // Checks
            if (!Multiplayer.IsServer())
                return;
        }

        // Recollision Check
        if (area == _recollisionCheck)
            return;

        // Check - Change Vertical/Horizontal
        if (area is player_wall)
            _left = !_left;
        else
            _up = !_up;

        // Set Recollision Check
        _recollisionCheck = area;
    }

    private void Ball_BodyEntered(Node2D body)
    {
        // Multiplayer Check
        if (Multiplayer.MultiplayerPeer != null)
        {
            // Checks
            if (!Multiplayer.IsServer())
                return;
        }

        // Recollision Check
        if (body == _recollisionCheck)
            return;

        // Change Horizontal
        _left = !_left;

        // Increase Speed
        if (body is player)
            _currentSpeed += SPEED_INCREASE;

        if (body is pong_ai)
            _currentSpeed += SPEED_INCREASE;

        // Set Recollision Check
        _recollisionCheck = body;
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
        _recollisionCheck = null;
        _currentSpeed = START_SPEED;
    }

    #endregion

    #region Functions

    private void CalculateMovement(float delta)
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

    #endregion
}
