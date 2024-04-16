using Godot;

public partial class pong_game : Node2D
{
    [ExportCategory("Scenes")]
    [Export]
    private PackedScene _playerScene;
    [Export]
    private PackedScene _aiScene;

    [ExportCategory("Game Nodes")]
    [Export]
    private Timer _gameStartTimer;
    [Export]
    private arena _arena;
    [Export]
    private ball _ball;
    [Export]
    private Marker2D _player1Spawn;
    [Export]
    private Marker2D _player2Spawn;
    [Export]
    private Area2D _goal1;
    [Export]
    private Area2D _goal2;

    [ExportCategory("UI Nodes")]
    [Export]
    private Label _gameStatus;
    [Export]
    private Label _gameInstructions;
    [Export]
    private Label _player1Label;
    [Export]
    private Label _player2Label;

    // Gameplay Types
    private const int _WINNING_SCORE = 5;
    private const int _GAME_START_COUNTDOWN = 5; // Seconds
    private const int _HIDE_STATUS_COUNTDOWN = 10; // Seconds
    
    private GameManager.GameModes _gameMode = GameManager.GameModes.SINGLEPLAYER;

    private int _currentGameCountdown = 0;
    private int _player1Score = 0;
    private int _player2Score = 0;
    private string _player1Name = "";
    private string _player2Name = "";

    private float _musicIntensity = 0.0f;

    public void Setup(GameManager.GameModes gameMode)
    {
        // Initalise
        _gameMode = gameMode;

        // Setup Players
        switch (gameMode)
        {
            case GameManager.GameModes.SINGLEPLAYER:
                // Spawn Objects
                SpawnPlayer("Player", _player1Spawn);
                SpawnAI(_player2Spawn);

                // Store Names
                _player1Name = "Player";
                _player2Name = "AI";
                break;

            case GameManager.GameModes.VERSUS:
                // Spawn Objects
                SpawnPlayer("Player 1", _player1Spawn);
                SpawnPlayer("Player 2", _player2Spawn);

                // Store Names
                _player1Name = "Player 1";
                _player2Name = "Player 2";
                break;

            case GameManager.GameModes.MULTIPLAYER:
                // Spawn Objects
                SpawnMultiplayerPlayer((int)MultiplayerManager.instance.GetHost().Id, MultiplayerManager.instance.GetHost().Id.ToString(), _player1Spawn);
                SpawnMultiplayerPlayer((int)MultiplayerManager.instance.GetOtherPlayer().Id, MultiplayerManager.instance.GetOtherPlayer().Id.ToString(), _player2Spawn);

                // Check
                if (!Multiplayer.IsServer())
                    return;

                // Store Names
                _player1Name = MultiplayerManager.instance.GetHost().Name;
                _player2Name = MultiplayerManager.instance.GetOtherPlayer().Name;
                break;
        }

        // Pause Ball
        _ball.ProcessMode = ProcessModeEnum.Disabled;

        // Setup Events
        _gameStartTimer.Timeout += GameStartTimer_Timeout;
        _goal1.AreaEntered += Goal1_AreaEntered;
        _goal2.AreaEntered += Goal2_AreaEntered;

        // Setup UI
        UpdateCountdownLabel();
        _player1Label.Text = $"Score: {_player1Score}";
        _player2Label.Text = $"Score: {_player2Score}";

        // Start Timer
        _gameStartTimer.Start();
    }

    public override void _Process(double delta)
    {
        // Checks
        if (Multiplayer.MultiplayerPeer != null)
        {
            if (!Multiplayer.IsServer())
                return;
        }

        if (_player1Score < _WINNING_SCORE && _player2Score < _WINNING_SCORE)
            return;

        // Input
        if (Input.IsActionJustPressed("game_restart"))
        {
            // Reset Points
            _player1Score = 0;
            _player2Score = 0;

            // Reset Game Countdown
            _currentGameCountdown = 0;

            // Reset Audio
            _musicIntensity = 0.0f;
            MusicManager.instance.SetIntensity(_musicIntensity);

            // Update UI
            UpdateCountdownLabel();

            _player1Label.Text = $"Score: {_player1Score}";
            _player2Label.Text = $"Score: {_player2Score}";
            _gameInstructions.Visible = false;

            // Start Timer
            _gameStartTimer.Start();
        }

        if (Input.IsActionJustPressed("game_exit"))
        {
            // TODO - Load Lobby Screen
        }
    }

    #region Events

    private void GameStartTimer_Timeout()
    {
        // Increment
        _currentGameCountdown++;

        // Check
        if (_currentGameCountdown < _GAME_START_COUNTDOWN)
        {
            // Update UI
            UpdateCountdownLabel();
        }
        else if (_currentGameCountdown >= _HIDE_STATUS_COUNTDOWN)
        {
            // Update UI
            _gameStatus.Text = "";

            // Stop Timer
            _gameStartTimer.Stop();
        }
        else
        {
            // Start Game
            _ball.ProcessMode = ProcessModeEnum.Inherit;

            // Update UI
            _gameStatus.Text = "Game Started!";
        }
    }

    private void Goal1_AreaEntered(Area2D area)
    {
        if (area == _ball)
        {
            // Increment Score
            _player2Score++;

            // Update UI
            _player2Label.Text = $"Score: {_player2Score}";

            // Handle Event
            if (_player2Score >= _WINNING_SCORE)
                HandleGameOver(_player2Name);
            else
                HandleAreaEntered();
        }
    }

    private void Goal2_AreaEntered(Area2D area)
    {
        if (area == _ball)
        {
            // Increment Score
            _player1Score++;

            // Update UI
            _player1Label.Text = $"Score: {_player1Score}";

            // Handle Event
            if (_player1Score >= _WINNING_SCORE)
                HandleGameOver(_player1Name);
            else
                HandleAreaEntered();
        }
    }

    #endregion

    #region Functions

    private void HandleAreaEntered()
    {
        // Reset Ball
        _ball.Reset();

        // Increase Intensity
        _musicIntensity += _musicIntensity < 1.0f ? 0.1f : 0.0f;
        MusicManager.instance.SetIntensity(_musicIntensity);
    }

    private void HandleGameOver(string playerName)
    {
        // Reset Ball
        _ball.Reset();

        // Pause Ball
        _ball.ProcessMode = ProcessModeEnum.Disabled;

        // Update UI
        _gameStatus.Text = $"{playerName} wins!";

        // Show Label
        _gameInstructions.Visible = true;
    }

    private void SpawnAI(Marker2D spawnPoint)
    {
        // Setup
        pong_ai ai = (pong_ai)_aiScene.Instantiate();

        // Setup Methods
        ai.Setup(_ball);

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
        player player = (player)_playerScene.Instantiate();

        // Assign Properties
        player.Name = name;
        player.Transform = spawnPoint.Transform;

        // Add to Tree
        _arena.AddChild(player);

        // Remove Marker
        spawnPoint.QueueFree();
    }

    private void SpawnMultiplayerPlayer(int id, string name, Marker2D spawnPoint)
    {
        // Setup
        player player = (player)_playerScene.Instantiate();

        // Assign Properties
        player.Name = name;
        player.Transform = spawnPoint.Transform;
        player.SetMultiplayerAuthority(id, true);

        // Add to Tree
        _arena.AddChild(player);

        // Remove Marker
        spawnPoint.QueueFree();
    }

    private void UpdateCountdownLabel()
        => _gameStatus.Text = $"Starting in {_GAME_START_COUNTDOWN - _currentGameCountdown}";

    #endregion
}
