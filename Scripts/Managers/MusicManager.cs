using Godot;

public partial class MusicManager : Node
{
    [ExportCategory("Nodes")]
    [Export]
    private Node _ovaniPlayer;

    [ExportCategory("Songs")]
    [Export]
    private Resource _mainMenuSong;
    [Export]
    private Resource _snakeSong;
    [Export]
    private Resource _breakoutSong;
    [Export]
    private Resource _pongSong;
    [Export]
    private Resource _tronSong;
    [Export]
    private Resource _platformerSong;

    private const float _DEFAULT_TRANSITION_TIME = 2.5f; // Seconds

    public static MusicManager instance;

    public override void _EnterTree()
        => instance = this;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Load Menu Music
        StartMenuMusic();
    }

    #region Public Functions

    public void StartMenuMusic()
    {
        // Set Intensify
        SetIntensity(0.5f);

        // Queue Song
        PlaySongName(_mainMenuSong, _DEFAULT_TRANSITION_TIME);
    }

    public void StartGameSong(GameManager.Games game)
    {
        // Set Intensify
        SetIntensity(0f);

        // Queue Song
        PlaySongName(GetSong(game), _DEFAULT_TRANSITION_TIME);
    }

    public void SetIntensity(float intensity)
        => _ovaniPlayer.CallDeferred("FadeIntensity", Mathf.Clamp(intensity, 0.0f, 1.0f));

    public void SetVolume(float volume)
        => _ovaniPlayer.CallDeferred("FadeVolume", volume);

    #endregion

    #region Private Functions

    private Resource GetSong(GameManager.Games game)
    {
        switch (game)
        {
            case GameManager.Games.SNAKE:
                return _snakeSong;

            case GameManager.Games.BREAKOUT:
                return _breakoutSong;

            case GameManager.Games.PONG:
                return _pongSong;

            case GameManager.Games.TRON:
                return _tronSong;

            case GameManager.Games.PLATFORMER:
                return _platformerSong;
        }
        return null;
    }

    private void PlaySongName(Resource song, float transitionTime = -1)
        => _ovaniPlayer.CallDeferred("PlaySongNow", song, transitionTime);

    #endregion
}
