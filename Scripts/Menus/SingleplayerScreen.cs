using Godot;

public partial class SingleplayerScreen : VBoxContainer
{
    [ExportCategory("Screens")]
    [Export]
    private Control _backScreen;

    [ExportCategory("Buttons")]
    [Export]
    private Button _snakeButton;
    [Export]
    private Button _breakoutButton;
    [Export]
    private Button _pongButton;
    [Export]
    private Button _platformerButton;
    [Export]
    private Button _backButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Assign Signals
        _snakeButton.Pressed += SnakeButton_Pressed;
        _breakoutButton.Pressed += BreakoutButton_Pressed;
        _pongButton.Pressed += PongButton_Pressed;
        _platformerButton.Pressed += PlatformerButton_Pressed;
        _backButton.Pressed += BackButton_Pressed;
    }

    private void SnakeButton_Pressed()
        => GameManager.instance.LoadGame(GameManager.Games.SNAKE);
    

    private void BreakoutButton_Pressed()
    {
        // TODO
    }

    private void PongButton_Pressed()
        => GameManager.instance.LoadGame(GameManager.Games.PONG, GameManager.GameModes.SINGLEPLAYER);

    private void PlatformerButton_Pressed()
        => GameManager.instance.LoadGame(GameManager.Games.PLATFORMER, GameManager.GameModes.SINGLEPLAYER);

    private void BackButton_Pressed()
    {
        Visible = false;
        _backScreen.Visible = true;
    }
}
