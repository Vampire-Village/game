using System;
using Mirror;
using ParrelSync;

public class VampireVillageNetwork : NetworkManager
{
    public static VampireVillageNetwork instance { get; private set; }

    public event Action<bool> OnNetworkStart;
    public event Action OnNetworkOnline;
    public event Action OnNetworkOffline;

    public override void Awake()
    {
        base.Awake();
        instance = this;
    }

    public override void Start()
    {
        base.Start();

        // Setup for ParrelSync.
    #if UNITY_EDITOR
        if (ClonesManager.IsClone())
            StartClient();
        else
            StartServer();
    #endif
    }

    #region Server Hooks

    public override void OnStartServer()
    {
        OnNetworkStart?.Invoke(true);
        GameLogger.LogServer("Started!");
    }

    #endregion

    #region Client Hooks

    public override void OnStartClient()
    {
        OnNetworkStart?.Invoke(false);
        GameLogger.LogClient("Connecting to the server...");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        OnNetworkOnline?.Invoke();
        GameLogger.LogClient("Successfully connected to the server!");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        OnNetworkOffline?.Invoke();
        GameLogger.LogClient("Disconnected from the server.");

        // TODO: Retry connecting instead.
        StopClient();
    }

    #endregion

    public override void OnApplicationQuit()
    {
        if (NetworkClient.isConnected)
        {
            StopClient();
            GameLogger.LogClient("Stopped.");
        }

        if (NetworkServer.active)
        {
            StopServer();
            GameLogger.LogServer("Stopped.");
        }
    }
}
