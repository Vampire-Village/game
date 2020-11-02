using Mirror;
using ParrelSync;

public class VampireVillageNetwork : NetworkManager
{
    public override void Start()
    {
        base.Start();
        showDebugMessages = false;

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
        GameLogger.LogServer("Started!");
    }

#endregion

#region Client Hooks

    public override void OnClientConnect(NetworkConnection conn)
    {
        GameLogger.LogClient("Successfully connected to the server!");
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
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
