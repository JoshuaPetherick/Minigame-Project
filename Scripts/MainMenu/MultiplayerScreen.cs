using Godot;

public partial class MultiplayerScreen : VBoxContainer
{
    [ExportCategory("Screens")]
    [Export]
    private Control _backScreen;

    [ExportCategory("Labels")]
    [Export]
    private MultiplayerLabel _player1;
    [Export]
    private MultiplayerLabel _player2;
    [Export]
    private MultiplayerLabel _player3;
    [Export]
    private MultiplayerLabel _player4;

    [ExportCategory("Buttons")]
    [Export]
    private Button _pongButton;
    [Export]
    private Button _tronButton;
    [Export]
    private Button _backButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Disable Buttons
        _pongButton.Disabled = true;
        _tronButton.Disabled = true;

        // Assign Signals
        _pongButton.Pressed += PongButton_Pressed;
        _tronButton.Pressed += TronButton_Pressed;
        _backButton.Pressed += BackButton_Pressed;

        // Multiplayer Signals
        if (MultiplayerManager.instance != null)
        {
            MultiplayerManager.instance.PlayerConnected += Instance_PlayerConnected;
            MultiplayerManager.instance.PlayerDisconnected += Instance_PlayerDisconnected;
            MultiplayerManager.instance.ServerDisconnected += Instance_ServerDisconnected;
        }
    }

    public override void _Process(double delta)
    {
        // Checks
        if (MultiplayerManager.instance is null)
            return;

        if (Multiplayer.MultiplayerPeer is null)
            return;

        if (!Multiplayer.IsServer())
            return;

        // Enable/Disable Buttons
        _pongButton.Disabled = !(MultiplayerManager.instance.Players.Count == 2);
        _tronButton.Disabled = !(MultiplayerManager.instance.Players.Count >= 2);
    }

    #region Events

    private void PongButton_Pressed()
    {
        // Check
        if (!Multiplayer.IsServer())
            return;

        // Start Game
        MultiplayerManager.instance.StartGameSession((int)GameManager.Games.PONG);
    }

    private void TronButton_Pressed()
    {
        // TODO
    }

    private void BackButton_Pressed()
    {
        // Disconnect
        MultiplayerManager.instance.Disconnect();

        // Clear UI
        ClearLabel(_player1);
        ClearLabel(_player2);
        ClearLabel(_player3);
        ClearLabel(_player4);

        // Change Screen
        Visible = false;
        _backScreen.Visible = true;
    }

    private void Instance_PlayerConnected(long id, string name)
    {
        if (id == 1)
        {
            _player1.PlayerId = id;
            _player1.Text = name;
            return;
        }
        else
        {
            if (_player2.PlayerId is null)
            {
                _player2.PlayerId = id;
                _player2.Text = name;
                return;
            }

            if (_player3.PlayerId is null)
            {
                _player3.PlayerId = id;
                _player3.Text = name;
                return;
            }

            if (_player4.PlayerId is null)
            {
                _player4.PlayerId = id;
                _player4.Text = name;
            }
        }
    }

    private void Instance_PlayerDisconnected(long id)
    {
        ClearLabelById(_player1, id);
        ClearLabelById(_player2, id);
        ClearLabelById(_player3, id);
        ClearLabelById(_player4, id);
    }

    private void Instance_ServerDisconnected()
    {
        // Clear UI
        ClearLabel(_player1);
        ClearLabel(_player2);
        ClearLabel(_player3);
        ClearLabel(_player4);

        // Change Screen
        Visible = false;
        _backScreen.Visible = true;
    }

    #endregion

    #region Functions

    private void ClearLabelById(MultiplayerLabel label, long id)
    {
        if (label.PlayerId == id)
            ClearLabel(label);
    }

    private void ClearLabel(MultiplayerLabel label)
    {
        label.PlayerId = null;
        label.Text = "...";
    }

    #endregion
}
