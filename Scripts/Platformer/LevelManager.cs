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

    private CanvasLayer _levelUI
    {
        get => GetNode<CanvasLayer>("LevelUI");
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

    public void ShowUI()
        => _levelUI.Visible = true;

    public void LoadNextLevel()
	{
        // Checks 
        if (_player.GlobalPosition == Vector2.Zero)
            return;

        // Move to Next Level
        if (!OnFinalLevel)
            _level++;

        // Load Level
        LoadLevel();
    }

    public void ReplayLevel()
    {
        // Checks 
        if (_player.GlobalPosition == Vector2.Zero)
            return;

        // Reload Level
        LoadLevel();
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

        // Reset UI
        _levelUI.Visible = false;
    }

    #endregion
}
