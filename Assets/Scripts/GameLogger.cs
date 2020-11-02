using UnityEngine;

public static class GameLogger
{
    public static void LogServer(string message)
    {
        Debug.Log("<color='red'>[SERVER]</color> " + message);
    }

    public static void LogClient(string message)
    {
        Debug.Log("<color='green'>[CLIENT]</color> " + message);
    }
}
