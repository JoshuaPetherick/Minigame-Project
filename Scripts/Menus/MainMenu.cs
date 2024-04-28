using Godot;

public partial class MainMenu : Control
{
    [Export]
    private Control _mainScreen;
    [Export]
    private Control _lobbyScreen;

    public void LoadLobbyScreen()
    {
        _mainScreen.Visible = false;
        _lobbyScreen.Visible = true;
    }
}
