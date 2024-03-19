using Godot;

public partial class GameManager : Node
{
	[ExportCategory("Scenes")]
	[Export]
	private PackedScene _mainMenu;
	[Export]
	private PackedScene _pongGame; 

    public static GameManager instance;
	public enum Games
	{
		SNAKE = 1,
		BREAKOUT = 2,
		PONG = 3,
		TRON = 4,
		PLATFORMER = 5
	}
    public enum GameModes
    {
        SINGLEPLAYER = 1,
        VERSUS = 2,
        MULTIPLAYER = 3
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// Initalise
		instance = this;

		// Load Menu
		AddChild(_mainMenu.Instantiate());
    }

	public void LoadGame(Games game, GameModes gameMode = GameModes.SINGLEPLAYER)
	{
		// Removes Current Node
		RemoveCurrentNode();

		// Setup Node
		Node gameNode = GetGameNode(game);

        // Add Game to Tree
        AddChild(gameNode);

        // Call Setup
        switch (game)
        {
            case Games.PONG:
                ((pong_game)gameNode).Setup(gameMode);
                break;
        }

        // Start Game Music
        MusicManager.instance.StartGameSong(game);
    }

    #region Functions

	private void RemoveCurrentNode()
		=> GetChild(0).QueueFree();

    private Node GetGameNode(Games game)
	{
		switch (game)
		{
			case Games.SNAKE:
				// TODO
				break;
            case Games.BREAKOUT:
                // TODO
                break;
            case Games.PONG:
                return _pongGame.Instantiate();
            case Games.TRON:
                // TODO
                break;
            case Games.PLATFORMER:
                // TODO
                break;
        }
		return null;
	}

    #endregion
}
