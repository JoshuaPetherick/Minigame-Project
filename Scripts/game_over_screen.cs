using Godot;
using System;

public partial class game_over_screen : Control
{
	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}

	public void _on_retry_pressed()
	{
        GetTree().ChangeSceneToFile("res://Scenes/SnakeGame.tscn");
    }
	
	
	
}
