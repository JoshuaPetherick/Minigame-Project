using Godot;
using System.IO;
using System.Linq;

public partial class snake : Node2D
{
	[Export]
	public Timer timer;
	[Export]
	private PackedScene bodyPiece;
	[Export]
	private Node2D snakeBody;

    public const int SNAKE_SIZE = 60;
    private const int _STARTING_BODY_COUNT = 3;
	public double SNAKE_SPEED_MINIMUM = 0.150;
	private const double _SNAKE_SPEED_MODIFIER = 0.025;
	public bool inverseControls = false;

	public enum DIRECTION
	{
		North = 0,
		South = 1,
		East = 2,
		West = 3
	};
	public DIRECTION _direction = DIRECTION.East;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Spawn Body Pieces
		for (int i = 1; i <= _STARTING_BODY_COUNT; i++) 
		{
            AddBodyPiece(new Vector2((-SNAKE_SIZE * i), 0));
        }

        // Connect to Signals
        timer.Timeout += Timer_Timeout;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		// Get Player Inputs

		if (inverseControls) 
		{
            if (Input.IsActionJustPressed("up") && _direction != DIRECTION.North)
                _direction = DIRECTION.South;
            else if (Input.IsActionJustPressed("down") && _direction != DIRECTION.South)
                _direction = DIRECTION.North;
            else if (Input.IsActionJustPressed("right") && _direction != DIRECTION.East)
                _direction = DIRECTION.West;
            else if (Input.IsActionJustPressed("left") && _direction != DIRECTION.West)
                _direction = DIRECTION.East;
        }

		if (Input.IsActionJustPressed("up") && _direction != DIRECTION.South)
			_direction = DIRECTION.North;
		else if (Input.IsActionJustPressed("down") && _direction != DIRECTION.North)
            _direction = DIRECTION.South;
        else if (Input.IsActionJustPressed("right") && _direction != DIRECTION.West)
            _direction = DIRECTION.East;
        else if (Input.IsActionJustPressed("left") && _direction != DIRECTION.East)
            _direction = DIRECTION.West;
    }

    #region Signals

    /// <summary>
    /// Ticks every time the snake should be moved. 
    /// </summary>
    private void Timer_Timeout()
    {
        // Setup
        Vector2 newPos = Vector2.Zero;

        // Get Children
        Node[] snakeBodyPieces = snakeBody.GetChildren().ToArray();

		// Get Snake Head Position
		Vector2 currentPos = ((Node2D)snakeBodyPieces[0]).Position;

        // Move head based on Direction
        switch (_direction)
        {
            case DIRECTION.North:
                newPos = new Vector2(currentPos.X, currentPos.Y - SNAKE_SIZE);
                break;
            case DIRECTION.South:
                newPos = new Vector2(currentPos.X, currentPos.Y + SNAKE_SIZE);;
                break;
            case DIRECTION.East:
                newPos = new Vector2(currentPos.X + SNAKE_SIZE, currentPos.Y);
                break;
            case DIRECTION.West:
                newPos = new Vector2(currentPos.X - SNAKE_SIZE, currentPos.Y);
                break;
        }

        // Assign New Position
        ((Node2D)snakeBodyPieces[0]).Position = newPos;

        // Loop 
        for (int i = (snakeBodyPieces.Length - 1); i > 0 ; i--)
		{
            // Check if the 1st body piece
			if (i == 1)
                ((Node2D)snakeBodyPieces[i]).Position = currentPos;
            else
            {
                ((Node2D)snakeBodyPieces[i]).Position = ((Node2D)snakeBodyPieces[(i - 1)]).Position;
            }

            //Change body rptation
            if(_direction == DIRECTION.North || _direction == DIRECTION.South)
            {
                ((Node2D)snakeBodyPieces[i]).GlobalRotationDegrees = 90;
            }
            else
            {
                ((Node2D)snakeBodyPieces[i]).GlobalRotationDegrees = 0;
            }		
        }
    }

    #endregion

    #region Functions

	public bool AreYouHere(Vector2 position)
	{
        // Get Children
        Node[] snakeBodyPieces = snakeBody.GetChildren().ToArray();

		// Check if in position
		foreach (Node2D node in snakeBodyPieces)
		{
			if (node.GlobalPosition.X < position.X + 30 &&
				node.GlobalPosition.X + SNAKE_SIZE > position.X &&
				node.GlobalPosition.Y < position.Y + 30 &&
				node.GlobalPosition.Y + SNAKE_SIZE > position.Y)
				return true;
		}

        return false;
	}

    public void AddBodyPiece()
    {
        // Get Body
        Node[] snakeBodyPieces = snakeBody.GetChildren().ToArray();

		// Get End Position
		Vector2 position = ((Node2D)snakeBodyPieces[(snakeBodyPieces.Length - 1)]).Position;

		// Add New Piece
		AddBodyPiece(position);

		// Amend Timer
		if (timer.WaitTime >= SNAKE_SPEED_MINIMUM)
			timer.WaitTime -= _SNAKE_SPEED_MODIFIER;
    }

    private void AddBodyPiece(Vector2 position)
	{
		// Setup
		string name = $"Snake Body {snakeBody.GetChildCount()}";

		// Spawn Body Piece
		Node2D piece = (Node2D)bodyPiece.Instantiate();

		// Set Properties
		piece.Name = name;
        piece.Position = position;

		// Add to Tree
		snakeBody.AddChild(piece);
	}

    private void RemoveAllBodyPieces()
    {
        //Remove all but head
        for(int i = 1; i < snakeBody.GetChildCount(); i++)
        {
            snakeBody.GetChild(i).QueueFree();
        }
    }

    private void RemoveBodyPiece()
    {
        snakeBody.GetChild(snakeBody.GetChildCount() - 1).QueueFree();
    }

    private void Restart()
    {
        RemoveAllBodyPieces();
        GetSnakeHead().Position = Vector2.Zero;
        _direction = DIRECTION.East;

        // Spawn Body Pieces
        for (int i = 1; i <= _STARTING_BODY_COUNT; i++)
        {
            AddBodyPiece(new Vector2((-SNAKE_SIZE * i), 0));
        }
    }

	public Area2D GetSnakeHead()
		=> (Area2D)snakeBody.GetChildren()[0];

    #endregion
}
