using Godot;

public partial class MainScreen : VBoxContainer
{
	[ExportCategory("Screens")]
	[Export]
	private Control _singlePlayerScreen;
    [Export]
    private Control _versusPlayerScreen;
    [Export]
    private Control _multiPlayerScreen;
    [Export]
    private Control _settingsScreen;

    [ExportCategory("Buttons")]
	[Export]
	private Button _singlePlayerButton;
    [Export]
    private Button _versusButton;
    [Export]
    private Button _multiPlayerButton;
    [Export]
    private Button _settingsButton;
    [Export]
    private Button _exitButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        // Set Properaties
        Visible = true;
        _singlePlayerScreen.Visible = false;
        _versusPlayerScreen.Visible = false;
        _multiPlayerScreen.Visible = false;
        _settingsScreen.Visible = false;

        // Assign Signals
        _singlePlayerButton.Pressed += SinglePlayerButton_Pressed;
        _versusButton.Pressed += VersusButton_Pressed;
        _multiPlayerButton.Pressed += MultiPlayerButton_Pressed;
        _settingsButton.Pressed += SettingsButton_Pressed;
        _exitButton.Pressed += ExitButton_Pressed;
    }

    private void SinglePlayerButton_Pressed()
    {
        Visible = false;
        _singlePlayerScreen.Visible = true;
    }

    private void VersusButton_Pressed()
    {
        Visible = false;
        _versusPlayerScreen.Visible = true;
    }

    private void MultiPlayerButton_Pressed()
    {
        Visible = false;
        _multiPlayerScreen.Visible = true;
    }

    private void SettingsButton_Pressed()
    {
        Visible = false;
        _settingsScreen.Visible = true;
    }

    private void ExitButton_Pressed()
        => GetTree().Quit();
}
