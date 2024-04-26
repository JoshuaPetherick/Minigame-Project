using Godot;
using System;
using System.Collections.Generic;

public partial class arena : Node2D
{
    [Export]
    private Node2D _playerParentNode;
    [Export]
    private Node2D _obstaclesParentNode;
    [Export]
    private Node2D _leftWalls;
    [Export]
    private Node2D _centerWalls;
    [Export]
    private Node2D _rightWalls;

    private const float _ROTATION_SPEED = 1.0f;
    private const float _FAST_ROTATION_SPEED = 45.0f;

    private const float _MIN_BALL_SIZE = 6.0f;
    private const float _MAX_BALL_SIZE = 10.0f;
    private const float _BALL_SHRINK_SPEED = 2f;

    private const float _MIN_PLAYER_SIZE = 50.0f;
    private const float _MAX_PLAYER_SIZE = 75.0f;
    private const float _PLAYER_SHRINK_SPEED = 12.5f;

    private const float _MIN_WALL_POSITION = 305.0f;
    private const float _MAX_WALL_POSITION = 455.0f;
    private const float _MIN_WALL_SIZE = 600.0f;
    private const float _MAX_WALL_SIZE = 900.0f;
    private const float _WALL_SHRINK_SPEED = 150.0f;

    private enum Mutations
	{
		ROTATE_ARENA,
        SHRINK_ARENA,
        SHINK_BALL,
        SHRINK_PLAYERS,
        ADD_OBSTACLES,
        //ADD_MOVING_OBSTACLE,
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
    private bool _isShrinkingArena = false;
    private bool _isShrinkingBall = false;
    private bool _isShrinkingPlayers = false;

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

        // Animated Mutations
        if (_isRotating90 || _isRotating180)
        {
            // Apply Rotation
            RotationDegrees += _FAST_ROTATION_SPEED * (float)delta;

            // Check
            if (RotationDegrees >= _targetRotation)
            {
                // Set to Target
                RotationDegrees = _targetRotation;

                // Clamp Rotation
                RotationDegrees = RotationDegrees >= 360.0f ? RotationDegrees - 360.0f : RotationDegrees;

                // Update Mutations
                _isRotating90 = false;
                _isRotating180 = false;

                // Resume Play
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Inherit;
            }

            // Escape
            return;
        }

        if (_isShrinkingBall)
        {
            // Setup
            ball ball = GetChild<ball>(0);

            // Apply Shrinking
            float newX = ball.Scale.X - (_BALL_SHRINK_SPEED * (float)delta);
            float newY = ball.Scale.Y - (_BALL_SHRINK_SPEED * (float)delta);
            ball.Scale = new Vector2(newX, newY);

            // Check
            if (ball.Scale.Y <= _MIN_BALL_SIZE)
            {
                // Clamp Size
                ball.Scale = new Vector2(_MIN_BALL_SIZE, _MIN_BALL_SIZE);

                // Update Mutations
                _isShrinkingBall = false;

                // Resume Play
                ball.ProcessMode = ProcessModeEnum.Inherit;
            }

            // Escape
            return;
        }

        if (_isShrinkingPlayers)
        {
            // Iterate
            foreach (Node2D player in _playerParentNode.GetChildren())
            {
                // Check
                if (player.IsQueuedForDeletion())
                    continue;

                // Apply Shrinking
                float newY = player.Scale.Y - (_PLAYER_SHRINK_SPEED * (float)delta);
                player.Scale = new Vector2(player.Scale.X, newY);

                // Check
                if (player.Scale.Y <= _MIN_PLAYER_SIZE)
                {
                    // Clamp Size
                    player.Scale = new Vector2(player.Scale.X, _MIN_PLAYER_SIZE);

                    // Update Mutations
                    _isShrinkingPlayers = false;

                    // Resume Play
                    GetChild<ball>(0).ProcessMode = ProcessModeEnum.Inherit;
                }
            }

            // Escape
            return;
        }

        if (_isShrinkingArena)
        {
            // Setup
            float sizeChange = (_WALL_SHRINK_SPEED * (float)delta);
            float positionChange = ((_WALL_SHRINK_SPEED / 2) * (float)delta);

            // Shrink Wall
            _centerWalls.Scale = new Vector2(_centerWalls.Scale.X - sizeChange, _centerWalls.Scale.Y);

            // Move Walls
            _leftWalls.Position = new Vector2(_leftWalls.Position.X + positionChange, _leftWalls.Position.Y);
            _rightWalls.Position = new Vector2(_rightWalls.Position.X - positionChange, _rightWalls.Position.Y);

            // Move Players
            _playerParentNode.GetChild<Node2D>(0).Position = new Vector2(_playerParentNode.GetChild<Node2D>(0).Position.X + positionChange, _playerParentNode.GetChild<Node2D>(0).Position.Y);
            _playerParentNode.GetChild<Node2D>(1).Position = new Vector2(_playerParentNode.GetChild<Node2D>(1).Position.X - positionChange, _playerParentNode.GetChild<Node2D>(1).Position.Y);

            // Check
            if (_centerWalls.Scale.X <= _MIN_WALL_SIZE)
            {
                // Clamp Size
                _centerWalls.Scale = new Vector2(_MIN_WALL_SIZE, _centerWalls.Scale.Y);

                // Clamp Position
                _leftWalls.Position = new Vector2(-_MIN_WALL_POSITION, _leftWalls.Position.Y);
                _rightWalls.Position = new Vector2(_MIN_WALL_POSITION, _rightWalls.Position.Y);

                // Clamp Players Position
                _playerParentNode.GetChild<Node2D>(0).Position = new Vector2(-_MIN_WALL_POSITION, _playerParentNode.GetChild<Node2D>(0).Position.Y);
                _playerParentNode.GetChild<Node2D>(1).Position = new Vector2(_MIN_WALL_POSITION, _playerParentNode.GetChild<Node2D>(1).Position.Y);

                // Update Mutations
                _isShrinkingArena = false;

                // Resume Play
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Inherit;
            }

            // Escape
            return;
        }

        // Regular Mutations
        if (_isRotating)
        {
            RotationDegrees += _ROTATION_SPEED * (float)delta;
            if (RotationDegrees >= 360.0f)
                RotationDegrees -= 360.0f;
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

    public string ApplyMutation()
    {
        // Check
        if (_mutations.Count == 0)
            return "";

        // Setup
        string result = "";
        int rngValue = rng.RandiRange(0, _mutations.Count - 1);

        // Get New Mutation
        Mutations mutation = _mutations[rngValue];

        // Apply Mutation
        switch (mutation)
        {
            case Mutations.ROTATE_ARENA:
                _isRotating = true;
                result = "Arena now Rotates!";
                break;

            case Mutations.SHRINK_ARENA:
                _isShrinkingArena = true;
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Disabled;
                result = "Arena has been Shrunk!";
                break;

            case Mutations.SHINK_BALL:
                _isShrinkingBall = true;
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Disabled;
                result = "Ball has been Shrunk!";
                break;

            case Mutations.SHRINK_PLAYERS:
                _isShrinkingPlayers = true;
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Disabled;
                result = "Players have been Shrunk!";
                break;

            case Mutations.ADD_OBSTACLES:
                _obstaclesParentNode.Visible = true;
                _obstaclesParentNode.ProcessMode = ProcessModeEnum.Inherit;
                result = "Obstacles added!";
                break;

            //case Mutations.ADD_MOVING_OBSTACLE:
            //    result = "Moving Obstacle added!";
            //    break;

            case Mutations.FLIP_ARENA_90:
                _isRotating90 = true;
                _targetRotation = RotationDegrees + 90.0f;
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Disabled;
                result = "Arena has been Rotated 90";
                break;

            case Mutations.FLIP_ARENA_180:
                _isRotating90 = true;
                _targetRotation = RotationDegrees + 180.0f;
                GetChild<ball>(0).ProcessMode = ProcessModeEnum.Disabled;
                result = "Arena has been Rotated 180";
                break;
        }

        // Remove from Array
        _mutations.Remove(mutation);

        // Result
        return result;
    }

    public void Reset()
    {
        // Reset Mutations
        LoadMutations();

        // Reset Arena
        RotationDegrees = 0.0f;
        _leftWalls.Position = new Vector2(-_MAX_WALL_POSITION, _leftWalls.Position.Y);
        _centerWalls.Scale = new Vector2(_MAX_WALL_SIZE, _centerWalls.Scale.Y);
        _rightWalls.Position = new Vector2(_MAX_WALL_POSITION, _rightWalls.Position.Y);

        // Reset Ball
        GetChild<ball>(0).Scale = new Vector2(_MAX_BALL_SIZE, _MAX_BALL_SIZE);

        // Reset Obstacles
        _obstaclesParentNode.Visible = false;
        _obstaclesParentNode.ProcessMode = ProcessModeEnum.Disabled;

        // Reset Players
        _playerParentNode.GetChild<Node2D>(0).Position = new Vector2(-_MAX_WALL_POSITION, _playerParentNode.GetChild<Node2D>(0).Position.Y);
        _playerParentNode.GetChild<Node2D>(1).Position = new Vector2(_MAX_WALL_POSITION, _playerParentNode.GetChild<Node2D>(1).Position.Y);

        _playerParentNode.GetChild<Node2D>(0).Scale = new Vector2(_playerParentNode.GetChild<Node2D>(0).Scale.X, _MAX_PLAYER_SIZE);
        _playerParentNode.GetChild<Node2D>(1).Scale = new Vector2(_playerParentNode.GetChild<Node2D>(1).Scale.X, _MAX_PLAYER_SIZE);
        
        // Reset Properties
        _isRotating = false;
    }

    #endregion
}
