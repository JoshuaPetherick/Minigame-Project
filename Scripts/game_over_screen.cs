using Godot;
using System;

public partial class game_over_screen : Control
{
	[Export]
	private Node2D snakeGame;
	public void _on_quit_pressed()
	{
        GameManager.instance.LoadMainMenu();
    }

	public void _on_retry_pressed()
	{
		Visible = false;
		snakeGame.ProcessMode = ProcessModeEnum.Always;
    }
	
	
	
}
