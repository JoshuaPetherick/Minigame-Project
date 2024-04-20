using Godot;
using System;
using System.Collections.Generic;

public partial class arena : Node2D
{
    private const float _ROTATION_SPEED = 0.02f;
    private const float _FAST_ROTATION_SPEED = 1.0f;

	private enum Mutations
	{
		ROTATE_ARENA,
        SHRINK_ARENA,
        SHRINK_PLAYERS,
        ADD_OBSTACLES,
        ADD_MOVING_OBSTACLE,
        FLIP_ARENA_90,
        FLIP_ARENA_180
    }
	private List<Mutations> _mutations = new List<Mutations>();
    private RandomNumberGenerator rng = new RandomNumberGenerator();

    // Mutation Properties
    private float _targetRotation = 0f;
	private bool _isRotating = false;
    private bool _isRotating90 = false;
    private bool _isRotating180 = false;

    public override void _Ready()
        => LoadMutations();

    public override void _PhysicsProcess(double delta)
    {
        // Checks
        if (Multiplayer.MultiplayerPeer != null)
        {
            // Checks
            if (!Multiplayer.IsServer())
                return;
        }

        // Mutations
        if (_isRotating90 || _isRotating180)
        {
            // Apply Rotation
            Rotation += _FAST_ROTATION_SPEED * (float)delta;

            // Check
            if (Rotation >= _targetRotation)
            {
                // Reset Mutations
                _isRotating90 = false;
                _isRotating180 = false;

                // Clamp Rotation
                Rotation = Rotation >= 360.0f ? Rotation - 360.0f : Rotation;

                // Resume Play
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Inherit;
            }

            // Escape
            return;
        }

        if (_isRotating)
        {
            Rotation += _ROTATION_SPEED * (float)delta;
            if (Rotation >= 360.0f)
                Rotation = 0.0f;
        }
    }

    #region Load

    public void LoadMutations()
    {
        _mutations.Clear();
        foreach (Mutations mutation in (Mutations[])Enum.GetValues(typeof(Mutations)))
        {
            _mutations.Add(mutation);
        }
    }

    #endregion

    #region Functions

    public void ApplyMutation()
    {
        // Check
        if (_mutations.Count == 0)
            return;
        
        // Get New Mutation
        int rngValue = rng.RandiRange(0, _mutations.Count - 1);
        Mutations mutation = _mutations[rngValue];

        // Apply Mutation
        switch (mutation)
        {
            case Mutations.ROTATE_ARENA:
                _isRotating = true;
                break;

            case Mutations.SHRINK_ARENA:
                break;

            case Mutations.SHRINK_PLAYERS:
                break;

            case Mutations.ADD_OBSTACLES:
                break;

            case Mutations.ADD_MOVING_OBSTACLE:
                break;

            case Mutations.FLIP_ARENA_90:
                _isRotating90 = true;
                _targetRotation = Rotation + 90.0f;
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Disabled;
                break;

            case Mutations.FLIP_ARENA_180:
                _isRotating90 = true;
                _targetRotation = Rotation + 180.0f;
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Disabled;
                break;
        }

        // Remove from Array
        _mutations.Remove(mutation);
    }

    public void Reset()
    {
        // Reset Mutations
        LoadMutations();

        // Reset Arena
        Rotation = 0.0f;

        // Reset Properties
        _isRotating = false;
    }

    #endregion
}
