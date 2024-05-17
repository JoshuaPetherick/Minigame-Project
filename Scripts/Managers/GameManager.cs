using Godot;

public partial class GameManager : Node
{
	[ExportCategory("Scenes")]
	[Export]
	private PackedScene _mainMenu;
	[Export]
	private PackedScene _pongGame;
    [Export]
    private PackedScene _snakeGame;
    [Export]
    private PackedScene _platformerGame;

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
            case Games.SNAKE:
                break;
        }

        // Start Game Music
        MusicManager.instance.StartGameSong(game);
    }

    public void LoadMainMenu()
    {
        // Removes Current Node
        RemoveCurrentNode();

        // Load Menu
        AddChild(_mainMenu.Instantiate());

        // Start Menu Music
        MusicManager.instance.StartMenuMusic();
    }

    public void LoadMultiplayerLobby()
    {
        // Removes Current Node
        RemoveCurrentNode();

        // Create Object
        MainMenu node = (MainMenu)_mainMenu.Instantiate();

        // Load Menu
        AddChild(node);

        // Load Lobby
        node.LoadLobbyScreen();

        // Start Menu Music
        MusicManager.instance.StartMenuMusic();
    }

    #region Functions

	private void RemoveCurrentNode()
		=> GetChild(0).QueueFree();

    private Node GetGameNode(Games game)
	{
		switch (game)
		{
			case Games.SNAKE:
                return _snakeGame.Instantiate();

            case Games.BREAKOUT:
                // TODO
                break;

            case Games.PONG:
                return _pongGame.Instantiate();

            case Games.TRON:
                // TODO
                break;

            case Games.PLATFORMER:
                return _platformerGame.Instantiate();
        }
		return null;
	}

    #endregion
}
