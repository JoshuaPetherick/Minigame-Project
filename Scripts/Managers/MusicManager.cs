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

    public const float DEFAULT_TRANSITION_TIME = 5.0f; // Seconds

    public static MusicManager instance;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initalise
        instance = this;

        // Reset Params
        SetIntensity(0, 0);

        // Load Menu Music
        StartMenuMusic();
    }

    #region Public Functions

    public void StartMenuMusic()
        => PlaySongName(_mainMenuSong, 0);

    public void StartGameSong(GameManager.Games game)
        => PlaySongName(GetSong(game), DEFAULT_TRANSITION_TIME);

    public void SetIntensity(float intensity, float transitionTime)
    => _ovaniPlayer.CallDeferred("FadeIntensity", Mathf.Clamp(intensity, 0.0f, 1.0f), transitionTime);

    public void SetVolume(float volume, float transitionTime)
        => _ovaniPlayer.CallDeferred("FadeVolume", volume, transitionTime);

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

            default:
                return null;
        }
    }

    private void PlaySongName(Resource song, float transitionTime = -1)
        => _ovaniPlayer.CallDeferred("PlaySongNow", song, transitionTime);

    #endregion
}
