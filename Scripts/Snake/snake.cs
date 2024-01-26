using Godot;
using System.Linq;

public partial class Snake : Node2D
{
	[Export]
	private string name;
	[Export]
	private Timer timer;
	[Export]
	private PackedScene bodyPiece;
	[Export]
	private Node2D snakeBody;

    public const int SNAKE_SIZE = 60;
    private const int _STARTING_BODY_COUNT = 3;

	private enum DIRECTION
	{
		North = 0,
		South = 1,
		East = 2,
		West = 3
	};
	private DIRECTION _direction = DIRECTION.East;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Spawn Body Pieces
		for (int i = 1; i <= _STARTING_BODY_COUNT; i++) 
		{
            AddBodyPiece(new Vector2((-SNAKE_SIZE * i), 0));
        }

		// Get Snake Head
		CharacterBody2D snakeHead = (CharacterBody2D)snakeBody.GetChildren()[0];

        // Connect to Signals
        timer.Timeout += Timer_Timeout;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		// Get Player Inputs
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
		// Get Children
		Node[] snakeBodyPieces = snakeBody.GetChildren().ToArray();

        // Loop 
        for (int i = (snakeBodyPieces.Length - 1); i >= 0 ; i--)
		{
			// Check if the head
			if (i == 0)
			{
				// Get Current Position
				Vector2 newPos = Vector2.Zero;
				Vector2 currentPos = ((Node2D)snakeBodyPieces[i]).Position;

                // Move based on Direction
                switch (_direction)
				{
					case DIRECTION.North:
                        newPos = new Vector2(currentPos.X, currentPos.Y - SNAKE_SIZE); 
						break;
                    case DIRECTION.South:
                        newPos = new Vector2(currentPos.X, currentPos.Y + SNAKE_SIZE);
                        break;
                    case DIRECTION.East:
                        newPos = new Vector2(currentPos.X + SNAKE_SIZE, currentPos.Y);
                        break;
                    case DIRECTION.West:
                        newPos = new Vector2(currentPos.X - SNAKE_SIZE, currentPos.Y);
                        break;
                }

                // Assign New Position
                ((Node2D)snakeBodyPieces[i]).Position = newPos;
            }
			else
				((Node2D)snakeBodyPieces[i]).Position = ((Node2D)snakeBodyPieces[(i - 1)]).Position;
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

	private void AddBodyPiece(Vector2 position)
	{
		// Spawn Body Piece
		Node2D piece = (Node2D)bodyPiece.Instantiate();

		// Set Position
		piece.Position = position;

		// Add to Tree
		snakeBody.AddChild(piece);
	}

    #endregion
}
