using Godot;

public partial class SnakeGame : Node2D
{
	[Export]
	private PackedScene appleScene;
	[Export]
	private Label scoreLabel;
	[Export]
	private Snake snake;

	private int apples = 0;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// Spawn Apple
		SpawnApple();

        // Update Label
        SetScore();

		// Signal Setup
		snake.GetSnakeHead().BodyEntered += SnakeHead_BodyEntered;
    }

    #region Signals

    private void SnakeHead_BodyEntered(Node2D body)
    {
		// Debug Logs
		GD.Print(body);
        GD.Print(body.Name);

        // Apple
        if (body is StaticBody2D)
		{
			// Add to Score
			apples++;

			// Update Label
			SetScore();

            // Remove Apple
            body.GetParent().QueueFree();

			// Spawn New Apple
			CallDeferred("SpawnApple");

			// Spawn New Piece
			snake.CallDeferred("AddBodyPiece");
		}

        // Snake Body
        if (body is CharacterBody2D)
		{
            // Game Over
            apples = 0;

            GetTree().ChangeSceneToFile("res://Scenes/game_over_screen.tscn");
            //GetTree().Quit();

            // Update Label
            SetScore();

        }
    }

    #endregion

    #region Functions

    private void SpawnApple()
	{
		// Setup
		string name = $"Apple {apples}";
		Vector2 newApplePosition = Vector2.Zero;

        // Spawn Apple Piece
        Node2D apple = (Node2D)appleScene.Instantiate();

        // Find Position
        while (true)
		{
			// Get Position
			int x = (rng.RandiRange(1, 31) * Snake.SNAKE_SIZE); // 32 = 1920 / SNAKE_SIZE
            int y = (rng.RandiRange(1, 17) * Snake.SNAKE_SIZE); // 18 = 1080 / SNAKE_SIZE

			// Offset by Apple Size
			x += 15;
			y += 15;

			// Populate Positions
			newApplePosition = new Vector2(x, y);

			// Check if can Spawn
			if (!snake.AreYouHere(newApplePosition))
				break;
        }

		// Set Properties
		apple.Name = name;
        apple.Position = newApplePosition;

        // Add to Tree
        AddChild(apple);
    }

	private void SetScore()
		=> scoreLabel.Text = $"{apples}";

    #endregion
}
