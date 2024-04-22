using Godot;
using System;

public partial class player_wall : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        BodyEntered += Player_wall_BodyEntered;
	}

    private void Player_wall_BodyEntered(Node2D body)
    {
        if (body is pong_player player)
        {
            player.CollidedWithWall();
        }
    }
}
