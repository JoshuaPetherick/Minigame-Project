using Godot;

public partial class SettingsScreen : VBoxContainer
{
    [ExportCategory("Screens")]
    [Export]
    private Control _backScreen;

    [ExportCategory("Buttons")]
    [Export]
    private Button _backButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Assign Signals
        _backButton.Pressed += BackButton_Pressed;
    }

    private void BackButton_Pressed()
    {
        Visible = false;
        _backScreen.Visible = true;
    }
}
