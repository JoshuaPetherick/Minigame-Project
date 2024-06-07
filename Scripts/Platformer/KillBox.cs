using Godot;

public partial class KillBox : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	    => BodyEntered += KillBox_BodyEntered;

    private void KillBox_BodyEntered(Node2D body)
    {
        if (body is Player player)
            player.Kill();
    }
}
