using Godot;

public partial class VersusScreen : VBoxContainer
{
    [ExportCategory("Screens")]
    [Export]
    private Control _backScreen;

    [ExportCategory("Buttons")]
    [Export]
    private Button _pongButton;
    [Export]
    private Button _backButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Assign Signals
        _pongButton.Pressed += PongButton_Pressed;
        _backButton.Pressed += BackButton_Pressed;
    }

    private void PongButton_Pressed()
        => GameManager.instance.LoadGame(GameManager.Games.PONG, GameManager.GameModes.VERSUS);

    private void BackButton_Pressed()
    {
        Visible = false;
        _backScreen.Visible = true;
    }
}
