using Godot;

public partial class UI_Level : CanvasLayer
{
	public Control UILevelCompleted { get; private set; }
    public Control UIPlayerDied { get; private set; }
    public Control ScreenShader
    {
        get => GetNode<Control>("ScreenShader");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        // Get Children
		UILevelCompleted = (Control)FindChild("UI_LevelCompleted");
        UIPlayerDied = (Control)FindChild("UI_PlayerDied");

        // Defaults
        UILevelCompleted.Visible = false;
        UIPlayerDied.Visible = false;
        ScreenShader.Visible = false;
    }
}
