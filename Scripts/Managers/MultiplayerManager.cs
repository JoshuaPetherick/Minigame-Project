using Godot;
using System;
using System.Collections.Generic;

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

    public Dictionary<long, string> Players = new Dictionary<long, string>();
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
        Error error = peer.CreateServer(_PORT, _MAX_CONNECTIONS);

        // Check
        if (error != Error.Ok)
            return error;

        // Setup Self
        Multiplayer.MultiplayerPeer = peer;
        Players.Add(1, name);
        _name = name;

        // Notify
        EmitSignal(SignalName.PlayerConnected, 1, name);

        // Result
        return Error.Ok;
    }

    public Error JoinGame(string address, string name)
    {
        // Setup
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        Error error = peer.CreateClient(address, _PORT);

        // Check
        if (error != Error.Ok)
            return error;
        
        // Setup Self
        Multiplayer.MultiplayerPeer = peer;
        _name = name;

        // Result
        return Error.Ok;
    }

    public void Disconnect()
    {
        // Close Connection 
        Multiplayer.MultiplayerPeer.Close();

        // Clear Data
        Players.Clear();

        // Set Null
        Multiplayer.MultiplayerPeer = null;
    }

    #endregion

    #region Events

    private void Multiplayer_PeerConnected(long id)
        => RpcId(id, "RegisterPlayer", _name);

    private void Multiplayer_PeerDisconnected(long id)
    {
        Players.Remove(id);
        EmitSignal(SignalName.PlayerDisconnected, id);
    }

    private void Multiplayer_ConnectedToServer()
    {
        int id = Multiplayer.GetUniqueId();
        Players.Add(id, _name);
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

    #region Functions

    [Rpc(mode: MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RegisterPlayer(string newPlayerName)
    {
        int id = Multiplayer.GetRemoteSenderId();
        Players.Add(id, newPlayerName);
        EmitSignal(SignalName.PlayerConnected, id, newPlayerName);
    }

    #endregion
}
