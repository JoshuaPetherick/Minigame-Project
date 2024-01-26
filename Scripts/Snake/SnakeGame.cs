using Godot;

public partial class SnakeGame : Node2D
{
	[Export]
	private PackedScene appleScene;
	[Export]
	private Label scoreLabel;
	[Export]
	private Snake snake;

	private int score = 0;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// Spawn Apple
		SpawnApple();

        // Update Label
        SetScore();
    }

	#region Functions

	private void SpawnApple()
	{
		// Setup
		Vector2 newApplePosition = Vector2.Zero;

        // Spawn Apple Piece
        Node2D apple = (Node2D)appleScene.Instantiate();

        // Find Position
        while (true)
		{
			// Get Position
			float x = (rng.RandfRange(1, 32) * Snake.SNAKE_SIZE); // 32 = 1920 / SNAKE_SIZE
			float y = (rng.RandfRange(1, 18) * Snake.SNAKE_SIZE); // 18 = 1080 / SNAKE_SIZE

			// Populate Positions
			newApplePosition = new Vector2(x, y);

			// Check if can Spawn
			if (!snake.AreYouHere(newApplePosition))
				break;
        }

		// Set Position
		apple.Position = newApplePosition;

        // Add to Tree
        AddChild(apple);
    }

	private void SetScore()
		=> scoreLabel.Text = $"Score: {score}";

    #endregion
}
