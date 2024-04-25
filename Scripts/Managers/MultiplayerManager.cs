using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Based off: https://docs.godotengine.org/en/stable/tutorials/networking/high_level_multiplayer.html#example-lobby-implementation
/// </summary>
public partial class MultiplayerManager : Node
{
	private const int _PORT = 7676;
	private const int _MAX_CONNECTIONS = 4;

    [Signal]
    public delegate void PlayerConnectedEventHandler(long id, string name);
    [Signal]
    public delegate void PlayerDisconnectedEventHandler(long id);
    [Signal]
    public delegate void ServerDisconnectedEventHandler();

    public List<PlayerInfo> Players = new List<PlayerInfo>();
    public string RoomCode { get; private set; }

    private string _name;

    public static MultiplayerManager instance;

    public override void _EnterTree()
        => instance = this;

    public override void _Ready()
    {
        // Setup Signals
        Multiplayer.PeerConnected += Multiplayer_PeerConnected;
        Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;
        Multiplayer.ConnectedToServer += Multiplayer_ConnectedToServer;
        Multiplayer.ConnectionFailed += Multiplayer_ConnectionFailed;
        Multiplayer.ServerDisconnected += Multiplayer_ServerDisconnected;
    }

    #region Public Functions

    public Error CreateGame(string name)
    {
        // Setup
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        string address = IP.GetLocalAddresses()[(IP.GetLocalAddresses().Length - 1)];

        // Setup Server
        Error error = peer.CreateServer(_PORT, _MAX_CONNECTIONS);
        
        // Check
        if (error != Error.Ok)
            return error;

        // Setup Self
        Multiplayer.MultiplayerPeer = peer;
        Players.Add(new PlayerInfo(1, name));

        _name = name;
        RoomCode = GetRoomCode(address);
        
        // Notify
        EmitSignal(SignalName.PlayerConnected, 1, name);

        // Result
        return Error.Ok;
    }

    public Error JoinGame(string roomCode, string name)
    {
        // Setup
        string address = GetAddress(roomCode);
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(address, _PORT);

        // Check
        if (error != Error.Ok)
            return error;
        
        // Setup Self
        Multiplayer.MultiplayerPeer = peer;
        _name = name;
        RoomCode = roomCode;

        // Result
        return Error.Ok;
    }

    public void StartGameSession(int game)
        => Rpc("StartGame", game);

    public void LoadLobbyScreen()
        => Rpc("LoadLobby");

    public void Disconnect()
    {
        // Close Connection 
        Multiplayer.MultiplayerPeer.Close();

        // Clear Data
        Players.Clear();

        // Set Null
        Multiplayer.MultiplayerPeer = null;
    }

    public PlayerInfo GetHost()
    {
        foreach (PlayerInfo player in Players)
        {
            if (player.Id == 1)
                return player;
        }
        return null;
    }

    public PlayerInfo GetOtherPlayer()
    {
        foreach (PlayerInfo player in Players)
        {
            if (player.Id != 1)
                return player;
        }
        return null;
    }

    #endregion

    #region Events

    private void Multiplayer_PeerConnected(long id)
        => RpcId(id, "RegisterPlayer", _name);

    private void Multiplayer_PeerDisconnected(long id)
    {
        Players.Remove(GetPlayerById(id));
        EmitSignal(SignalName.PlayerDisconnected, id);
    }

    private void Multiplayer_ConnectedToServer()
    {
        int id = Multiplayer.GetUniqueId();
        Players.Add(new PlayerInfo(id, _name));
        EmitSignal(SignalName.PlayerConnected, id, _name);
    }

    private void Multiplayer_ConnectionFailed()
        => Multiplayer.MultiplayerPeer = null;

    private void Multiplayer_ServerDisconnected()
    {
        Multiplayer.MultiplayerPeer = null;
        Players.Clear();
        EmitSignal(SignalName.ServerDisconnected);
    }

    #endregion

    #region RPC 

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RegisterPlayer(string newPlayerName)
    {
        int id = Multiplayer.GetRemoteSenderId();
        Players.Add(new PlayerInfo(id, newPlayerName));
        EmitSignal(SignalName.PlayerConnected, id, newPlayerName);
    }

    [Rpc(mode: MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void StartGame(int game)
        => GameManager.instance.LoadGame((GameManager.Games)game, GameManager.GameModes.MULTIPLAYER);

    [Rpc(mode: MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void LoadLobby()
        => GameManager.instance.LoadMultiplayerLobby();

    #endregion

    #region Functions

    private PlayerInfo GetPlayerById(long id)
    {
        foreach (PlayerInfo player in Players)
        {
            if (player.Id == id) 
                return player;
        }
        return null;
    }

    private string GetRoomCode(string ipAddress)
    {
        string result = "";
        foreach (string address in ipAddress.Split('.'))
        {
            int val = int.Parse(address);
            string hex = val.ToString("X2");

            result += hex;
        }
        return Reverse(result);
    }

    private string GetAddress(string roomCode)
    {
        string result = "";
        string actualRoomCode = Reverse(roomCode); 
        for (int i = 0; i < actualRoomCode.Length; i += 2)
        {
            int value = Convert.ToInt32($"0x{actualRoomCode[i]}{actualRoomCode[(i + 1)]}", 16);
            result += string.IsNullOrWhiteSpace(result) ? "" : "." + value.ToString();
        }
        return result;
    }

    private string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    #endregion
}
