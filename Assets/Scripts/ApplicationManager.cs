using UnityEngine;

/// <summary>
/// Handles getting and setting data that persists when the game is closed and opened again.
/// </summary>
public static class ApplicationManager
{
    private const string k_PlayerNameKey = "PlayerName";

    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString(k_PlayerNameKey);
    }

    public static void SetPlayerName(string name)
    {
        PlayerPrefs.SetString(k_PlayerNameKey, name);
    }
}
