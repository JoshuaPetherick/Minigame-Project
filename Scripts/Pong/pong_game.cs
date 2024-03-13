using Godot;

public partial class pong_game : Node2D
{
    [ExportCategory("Scenes")]
    [Export]
    private PackedScene _singleplayerScene;
    [Export]
    private PackedScene _multiplayerScene;
    [Export]
    private PackedScene _aiScene;

    [ExportCategory("Nodes")]
    [Export]
    private arena _arena;
    [Export]
    private Marker2D _player1Spawn;
    [Export]
    private Marker2D _player2Spawn;
    [Export]
    private Area2D _goal1;
    [Export]
    private Area2D _goal2;
    [Export]
    private Node _ovaniSoundPlayer;
    [Export]
    private Label _player1Label;
    [Export]
    private Label _player2Label;

    // Gameplay Types
    public enum GameModes
    {
        SINGLEPLAYER = 1,
        VERSUS = 2,
        MULTIPLAYER = 3
    }
    private GameModes _gameMode = GameModes.SINGLEPLAYER;

    private int _player1Score = 0;
    private int _player2Score = 0;

    private float _musicIntensity = 0f;

    /// <summary>
    /// Used for Testing!
    /// </summary>
    public override void _Ready()
        => Setup(GameModes.SINGLEPLAYER);

    // Should be called when the node enters the scene tree for the first time.
    public void Setup(GameModes gameMode)
    {
        // Initalise
        _gameMode = gameMode;

        // Setup Players
        switch (gameMode)
        {
            case GameModes.SINGLEPLAYER:
                SpawnPlayer("Player", _player1Spawn);
                SpawnAI(_player2Spawn);
                break;

            case GameModes.VERSUS:
                SpawnPlayer("Player 1", _player1Spawn);
                SpawnPlayer("Player 2", _player2Spawn);
                break;

            case GameModes.MULTIPLAYER:
                // TODO
                break;
        }

        // Setup Events
        _goal1.AreaEntered += Goal1_AreaEntered;
        _goal2.AreaEntered += Goal2_AreaEntered;

        // Setup Music
        _ovaniSoundPlayer.SetDeferred("Intensity", _musicIntensity);

        // Setup UI
        _player1Label.Text = $"Score: {_player1Score}";
        _player2Label.Text = $"Score: {_player2Score}";
    }

    private void Goal1_AreaEntered(Area2D area)
    {
        // Increment Scroe
        _player2Score++;

        // Update UI
        _player2Label.Text = $"Score: {_player2Score}";

        // Handle Event
        HandleAreaEntered(area);
    }

    private void Goal2_AreaEntered(Area2D area)
    {
        // Increment Scroe
        _player1Score++;

        // Update UI
        _player1Label.Text = $"Score: {_player1Score}";

        // Handle Event
        HandleAreaEntered(area);
    }

    #region Functions

    private void HandleAreaEntered(Area2D area)
    {
        if (area is ball ball)
        {
            // Reset Ball
            ball.Reset();

            // Increase Intensity
            _musicIntensity += _musicIntensity < 1 ? 0.1f : 0.0f;
            _ovaniSoundPlayer.SetDeferred("Intensity", _musicIntensity);
        }
    }

    private void SpawnAI(Marker2D spawnPoint)
    {
        // Setup
        pong_ai ai = (pong_ai)_aiScene.Instantiate();

        // Setup Methods
        ai.Setup(_arena.GetChild<ball>(0));

        // Assign Properties
        ai.Name = "AI";
        ai.Transform = spawnPoint.Transform;

        // Add to Tree
        _arena.AddChild(ai);

        // Remove Marker
        spawnPoint.QueueFree();
    }

    private void SpawnPlayer(string name, Marker2D spawnPoint)
    {
        // Setup
        player player = (player)_singleplayerScene.Instantiate();

        // Assign Properties
        player.Name = name;
        player.Transform = spawnPoint.Transform;

        // Add to Tree
        _arena.AddChild(player);

        // Remove Marker
        spawnPoint.QueueFree();
    }

    #endregion
}
