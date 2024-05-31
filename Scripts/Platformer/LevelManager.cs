using Godot;

public partial class LevelManager : Node2D
{
	[Export]
	private PackedScene[] _levels;
    [Export]
    private int _currentLevel = 1;

    public static LevelManager instance;

    private Node2D _level
    {
        get => GetNode<Node2D>("Level");
    }

    private Player _player
    {
        get => GetNode<Player>("Player");
    }

    public override void _EnterTree()
        => instance = this;

    public override void _Ready()
        => LoadLevel();

    #region Public Functions

    public void LoadNextLevel()
	{
        // Checks 
        if (_player.GlobalPosition == Vector2.Zero)
            return;

		GD.Print("Loading Next Level...");
        LoadLevel();
    }

    #endregion

    #region Functions

    private void LoadLevel() 
    {
        // Setup
        int level = _currentLevel - 1;

        // Clear Existing
        if (_level.GetChildCount() > 0)
            _level.GetChild(0).QueueFree();

        // Instantiate New Level
        _level.AddChild(_levels[level].Instantiate());

        // Reset Player
        _player.Reset();
    }

    #endregion
}
