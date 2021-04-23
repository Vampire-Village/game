using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using VampireVillage.Network;
using Mirror;

public class MeetingManager : NetworkBehaviour
{
#region Properties
    private GameManager gameManager;
    private List<GameObject> playerList;
    public GameObject warpPointGroup;
    private readonly List<GameObject> warpPoints = new List<GameObject>();
    


    [SerializeField]
    private GameObject chatButton;
#endregion

#region Unity Methods
    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        playerList = gameManager.players.ToList();
        gameManager.OnPlayerListUpdated.AddListener(updatePlayerList);
        
        foreach (Transform warpPoint in warpPointGroup.transform)
            warpPoints.Add(warpPoint.gameObject);
    }
    private void updatePlayerList()
    {
        playerList = gameManager.players.ToList();
        
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
        playerList = gameManager.players.ToList();
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

        // TODO: Disable player movement. should be done
        // TODO: Play sound effect
        // TODO: Wait 1-2 seconds
        //if(GamePlayer.local.role != Role.VampireLord)
        //{
            GamePlayer.local.GetComponent<Controller>().moveable = false;
            GamePlayer.local.gameObject.transform.position = warpPoints[playerList.IndexOf(GamePlayer.local.gameObject)].transform.position;

        //}
        
        
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
