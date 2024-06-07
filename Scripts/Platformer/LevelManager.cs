using Godot;

public partial class LevelManager : Node2D
{
    [Export]
    private int _level = 1;
	[Export]
	private PackedScene[] _levels;

    public static LevelManager instance;

    private Node2D _levelNode
    {
        get => GetNode<Node2D>("Level");
    }

    private Player _player
    {
        get => GetNode<Player>("Player");
    }

    private UI_Level _levelUI
    {
        get => GetNode<UI_Level>("UI_Level");
    }

    public bool OnFinalLevel
    {
        get => _levels.Length == _level;
    }

    public override void _EnterTree()
        => instance = this;

    public override void _Ready()
        => LoadLevel();

    #region Public Functions

    public void LevelCompleted()
    {
        // Checks 
        if (_player.GlobalPosition == Vector2.Zero)
            return;

        // Show/Hide UI
        _levelUI.UILevelCompleted.Visible = true;
        _levelUI.UIPlayerDied.Visible = false;
        _levelUI.ScreenShader.Visible = true;
    }

    public void PlayerDied()
    {
        // Checks 
        if (_player.GlobalPosition == Vector2.Zero)
            return;

        // Show/Hide UI
        _levelUI.UILevelCompleted.Visible = false;
        _levelUI.UIPlayerDied.Visible = true;
        _levelUI.ScreenShader.Visible = true;
    }

    public void LoadNextLevel()
	{
        // Move to Next Level
        if (!OnFinalLevel)
            _level++;

        // Load Level
        LoadLevel();

        // Show/Hide UI
        _levelUI.UILevelCompleted.Visible = false;
        _levelUI.UIPlayerDied.Visible = false;
        _levelUI.ScreenShader.Visible = false;
    }

    public void ReplayLevel()
    {
        // Reload Level
        LoadLevel();

        // Show/Hide UI
        _levelUI.UILevelCompleted.Visible = false;
        _levelUI.UIPlayerDied.Visible = false;
        _levelUI.ScreenShader.Visible = false;
    }

    #endregion

    #region Functions

    private void LoadLevel() 
    {
        // Setup
        int level = _level - 1;

        // Clear Existing
        if (_levelNode.GetChildCount() > 0)
            _levelNode.GetChild(0).QueueFree();

        // Instantiate New Level
        _levelNode.AddChild(_levels[level].Instantiate());

        // Reset Player
        _player.Reset();
    }

    #endregion
}
