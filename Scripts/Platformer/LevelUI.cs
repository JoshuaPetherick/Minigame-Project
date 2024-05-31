using Godot;

public partial class LevelUI : VBoxContainer
{
	private Button _next
	{
		get => GetNode<Button>("Next");
	}

    private Button _replay
    {
        get => GetNode<Button>("Replay");
    }

    private Button _exit
    {
        get => GetNode<Button>("Exit");
    }

	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        _next.Pressed += Next_Pressed;
        _replay.Pressed += Replay_Pressed;
        _exit.Pressed += Exit_Pressed;
    }

    #region Events

    private void Next_Pressed()
    {
        // Load Next Level
        LevelManager.instance.LoadNextLevel();

        // Check if now on Final Level
        _next.Visible = !LevelManager.instance.OnFinalLevel;
    }

    private void Replay_Pressed()
        => LevelManager.instance.ReplayLevel();

    private void Exit_Pressed()
        => GameManager.instance.LoadMainMenu();

    #endregion
}
