using Godot;

public partial class ConnectionScreen : VBoxContainer
{
    [ExportCategory("Screens")]
    [Export]
    private Control _backScreen;

    [ExportCategory("Buttons")]
    [Export]
    private Button _hostButton;
    [Export]
    private LineEdit _ipAddress;
    [Export]
    private Button _connectButton;
    [Export]
    private Button _backButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Assign Signals
        _hostButton.Pressed += HostButton_Pressed;
        _connectButton.Pressed += ConnectButton_Pressed;
        _backButton.Pressed += BackButton_Pressed;
    }

    private void HostButton_Pressed()
    {
        // TODO
    }

    private void ConnectButton_Pressed()
    {
        // TODO
    }

    private void BackButton_Pressed()
    {
        Visible = false;
        _backScreen.Visible = true;
    }
}
