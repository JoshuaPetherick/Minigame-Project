using Godot;

public partial class Signpost : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	    => BodyEntered += Signpost_BodyEntered;

    private void Signpost_BodyEntered(Node2D body)
    {
        if (body is Player)
            LevelManager.instance.ShowUI();
    }
}
