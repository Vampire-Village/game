using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireVillage.Network;
using Mirror;

public class MeetingManager : NetworkBehaviour
{
#region Properties
    private GameManager gameManager;

    [SerializeField]
    private GameObject chatButton;
#endregion

#region Unity Methods
    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }
#endregion

#region Server Methods
#if UNITY_SERVER || UNITY_EDITOR
    /// <summary>
    /// Starts a meeting at the town hall.
    /// Gets called when a player interacted with the bell or when it's day time.
    /// </summary>
    public void StartMeeting()
    {
        // Stop the night cycle.
        gameManager.StopNight();

        // Call meeting start on all clients.
        RpcOnMeetingStart();

        // TODO: Initialize meeting stuff.
    }

    /// <summary>
    /// Ends the meeting and decides who to kick.
    /// Gets called when the every player has voted or if the meeting time runs out.
    /// </summary>
    public void StopMeeting()
    {
        // TODO: Calculate vote and announce result.

        // TODO: Kill the voted-off player.

        // Restart the night cycle.
        gameManager.StartNight();

        // Call meeting end on all clients.
        RpcOnMeetingEnd();
    }
#endif
#endregion

#region Client Methods
    /// <summary>
    /// Called on the client when meeting is starting.
    /// </summary>
    [ClientRpc]
    private void RpcOnMeetingStart()
    {
        GameLogger.LogClient("A meeting is called!");

        // TODO: Disable player movement.

        // TODO: Show UI like Among Us emergency meeting announcement?

        // Show the chat button.
        chatButton.SetActive(true);
    }

    [ClientRpc]
    private void RpcOnMeetingEnd()
    {
        GameLogger.LogClient("The meeting has ended.");

        // TODO: Enable player movement.

        // Hide the chat button.
        chatButton.SetActive(false);
    }
#endregion
}
