using Godot;
using System;

public partial class main_menu : Control
{
    public void _on_exit_pressed()
    {
        GetTree().Quit();
    }
    public void _on_start_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/SnakeGame.tscn");
    }
    
}
