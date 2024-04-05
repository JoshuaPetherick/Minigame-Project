using Godot;

public partial class ConnectionScreen : VBoxContainer
{
    [ExportCategory("Screens")]
    [Export]
    private Control _backScreen;
    [Export]
    private Control _lobbyScreen;

    [ExportCategory("Buttons")]
    [Export]
    private LineEdit _playerName;
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
        // Checks
        if (string.IsNullOrWhiteSpace(_playerName.Text))
            return;

        // Create Game
        MultiplayerManager.instance.CreateGame(_playerName.Text);

        // Check
        if (Multiplayer.MultiplayerPeer is null)
            return;

        // Next Screen
        Visible = false;
        _lobbyScreen.Visible = true;
    }

    private void ConnectButton_Pressed()
    {
        // Checks
        if (string.IsNullOrWhiteSpace(_ipAddress.Text))
            return;

        if (string.IsNullOrWhiteSpace(_playerName.Text))
            return;

        // Join Game
        MultiplayerManager.instance.JoinGame(_ipAddress.Text, _playerName.Text);

        // Check
        if (Multiplayer.MultiplayerPeer is null)
            return;

        // Next Screen
        Visible = false;
        _lobbyScreen.Visible = true;
    }

    private void BackButton_Pressed()
    {
        Visible = false;
        _backScreen.Visible = true;
    }
}
