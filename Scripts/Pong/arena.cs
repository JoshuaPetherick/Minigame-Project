using Godot;
using System;

public partial class arena : Node2D
{
	[Export]
	private Area2D goal1;
    [Export]
    private Area2D goal2;
    [Export]
    private Node _ovaniSoundPlayer;

    public int Player1Score { get; private set; } = 0;
    public int Player2Score { get; private set; } = 0;
    private float _musIntensity = 0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        // Setup Events
        goal1.AreaEntered += Goal1_AreaEntered;
        goal2.AreaEntered += Goal2_AreaEntered;

        // Setup Music
        _ovaniSoundPlayer.SetDeferred("Intensity", _musIntensity);
    }

    private void Goal1_AreaEntered(Area2D area)
    {
        // Increment Scroe
        Player1Score++;

        // Handle Event
        HandleAreaEntered(area);
    }

    private void Goal2_AreaEntered(Area2D area)
    {
        // Increment Scroe
        Player2Score++;

        // Handle Event
        HandleAreaEntered(area);
    }

    #region Functions

    private void HandleAreaEntered(Area2D area)
    {
        if (area is ball ball)
        {
            // Reset Ball
            ball.Reset();

            // Increase Intensity
            _musIntensity += _musIntensity < 1 ? 0.1f : 0.0f;
            _ovaniSoundPlayer.SetDeferred("Intensity", _musIntensity);
        }
    }

    #endregion
}
