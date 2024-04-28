using Godot;

public partial class SnakeGame : Node2D
{
	[Export]
	private PackedScene appleScene;
	[Export]
	private Label scoreLabel;
	[Export]
	private snake snake;
	[Export]
	private Control dead;

	public int apples = 0;
	float musIntensity = 0f;
	int mutation;
	bool removedMutation = false;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		dead.Visible = false;
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

		// Collision
		if (body is StaticBody2D)
		{
			//Wall
			if (body.Name.ToString().Contains("Wall"))
			{
				//Screen size = 1920 x 1080

				Vector2 oldPosition = snake.GetSnakeHead().Position;

				Node2D wall = body.GetParent<Node2D>();
				if (wall.Name.ToString().Contains("Left"))
					snake.GetSnakeHead().Position = new Vector2(oldPosition.X + 1920, oldPosition.Y);
				if (wall.Name.ToString().Contains("Right"))
					snake.GetSnakeHead().Position = new Vector2(oldPosition.X - 1920, oldPosition.Y);
				if (wall.Name.ToString().Contains("Top"))
					snake.GetSnakeHead().Position = new Vector2(oldPosition.X, oldPosition.Y + 1080);
				if (wall.Name.ToString().Contains("Bottom"))
					snake.GetSnakeHead().Position = new Vector2(oldPosition.X, oldPosition.Y - 1080);

				return;
			}

			// Add to Score
			apples++;

			// Update Label
			SetScore();

			// Remove Apple
			body.GetParent().QueueFree();

			// Spawn New Apple
			CallDeferred("SpawnApple");

			//increment music intensity
			musIntensity += 0.05f;
			if (musIntensity < 1f) MusicManager.instance.SetIntensity(musIntensity);

			//reset controls & settings
			snake.inverseControls = false;
			removedMutation = false;

			//mutation

			if (apples >= 15)
			{
				snake.timer.WaitTime = snake.SNAKE_SPEED_MINIMUM;
				mutation = rng.RandiRange(1, 6);
				switch (mutation)
				{
					//double growth
					case 1:
						snake.CallDeferred("AddBodyPiece");
						break;
					//Anti growth
					case 2:
						snake.CallDeferred("RemoveBodyPiece");
						removedMutation = true;
						break;
					//Slow speed
					case 3:
						snake.timer.WaitTime = snake.timer.WaitTime + snake.timer.WaitTime;
						break;
					//double speed
					case 4:
						snake.timer.WaitTime = snake.timer.WaitTime / 2;
						break;
					//inverse controls
					case 5:
						snake.timer.WaitTime += 0.035;
						snake.inverseControls = true;
						break;
					//double apple
					case 6:
						CallDeferred("SpawnApple");
						break;
				}
			}
			// Spawn New Piece
			if (!removedMutation) 
				snake.CallDeferred("AddBodyPiece");
			
			removedMutation = false;
			SetScore();
		}

		// Snake Body
		if (body is CharacterBody2D)
		{
			ResetGame();
		}
	}

	#endregion

	#region Functions

	private void ResetGame()
	{
		//Stop processes
		CallDeferred(Node2D.MethodName.SetProcessMode, (int)ProcessModeEnum.Disabled);

		// Reset Values
		apples = 0;
		snake.inverseControls = false;
        snake.timer.WaitTime = 0.5;
		mutation = 0;

        // Update Score Label
        SetScore();

		//Turn off music (?)
		MusicManager.instance.SetIntensity(0f);

		//Restart snake
		snake.CallDeferred("Restart");

		//Show game over screen
		dead.Visible = true;
	}

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
			int x = (rng.RandiRange(1, 31) * snake.SNAKE_SIZE); // 32 = 1920 / SNAKE_SIZE
			int y = (rng.RandiRange(1, 17) * snake.SNAKE_SIZE); // 18 = 1080 / SNAKE_SIZE

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

	//For debugging (could stay?)
	private string GetMutationName(int mutation)
	{
		switch (mutation)
		{
			case 0:
				return "";
			case 1:
				return "double growth";
            case 2:
				return "anti growth";
            case 3:
				return "slow";
            case 4:
				return "double speed";
            case 5:
				return "inverse";
            case 6:
				return "apple";
        }
		return "";
	}

	private void SetScore()
		=> scoreLabel.Text = $"{apples}" + " " + $"{GetMutationName(mutation)}";

    #endregion
}
