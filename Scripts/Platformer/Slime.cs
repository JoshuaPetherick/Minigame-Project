using Godot;

public partial class Slime : CharacterBody2D
{
	public const float Speed = 300.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// TODO: Move Logic Here

		Velocity = velocity;
		MoveAndSlide();
	}
}
