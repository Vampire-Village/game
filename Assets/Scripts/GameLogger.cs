using UnityEngine;
using VampireVillage.Network;

public static class GameLogger
{
    public static void LogServer(object message)
    {
        Debug.Log($"<color='red'>[SERVER]</color> {message}\n");
    }

    public static void LogServer(object message, ServerPlayer player)
    {
        LogServer($"{message}\nPlayer ID: {player.id}\nConnection ID: {player.connectionId}");
    }

    public static void LogServer(object message, Client client)
    {
        LogServer($"{message}\nPlayer ID: {client.playerId}");
    }

    public static void LogClient(object message)
    {
        Debug.Log($"<color='green'>[CLIENT]</color> {message}\n");
    }

    public static void LogClient(object message, Client client)
    {
        LogClient($"{message}\nPlayer ID: {client.playerId}");
    }
}
