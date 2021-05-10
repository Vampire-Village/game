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
    public GameObject lightBeamGroup;
    private readonly List<GameObject> lightBeams = new List<GameObject>();
    private Dictionary<int, int> meetingTally = new Dictionary<int, int>();
    


    [SerializeField]
    private GameObject chatButton = null;
#endregion

#region Unity Methods
    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        playerList = gameManager.players.ToList();
        gameManager.OnPlayerListUpdated.AddListener(updatePlayerList);
        
        foreach (Transform warpPoint in warpPointGroup.transform)
            warpPoints.Add(warpPoint.gameObject);
        foreach (Transform lightBeam in lightBeamGroup.transform)
            lightBeams.Add(lightBeam.gameObject);
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
        // TODO: Add skip vote
        meetingTally = new Dictionary<int, int>();
        for (var i = 0; i < playerList.Count; i++)
        {
            GamePlayer playerReference = playerList[i].GetComponent<GamePlayer>();
            if (playerReference.role != Role.VampireLord || playerReference.role != Role.Ghost)
            {
                meetingTally.Add(i, 0);
            }
        }
        // TODO: Start timer to end meeting
    }

    public int TallyAllVotes()
    {
        int greatestIndex = -1;
        int greatestValue = 0;
        foreach(int i in meetingTally.Keys)
        {
            if(meetingTally[i] > greatestValue)
            {
                greatestIndex = i;
                greatestValue = meetingTally[i];
            }
            else if(meetingTally[i] == greatestValue)
            {
                greatestIndex = -1;
            }
        }
        return greatestIndex;
    }

    /// <summary>
    /// Ends the meeting and decides who to kick.
    /// Gets called when the every player has voted or if the meeting time runs out.
    /// </summary>
    public void StopMeeting()
    {
        // TODO: Calculate vote and announce result.
        int killTarget = TallyAllVotes();
        GameLogger.LogClient(killTarget);
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
        // TODO: Have the code skip the vampire without visual issues
        //if(GamePlayer.local.role != Role.VampireLord)
        //{
            int playerIndex = playerList.IndexOf(GamePlayer.local.gameObject);
            GamePlayer.local.GetComponent<Controller>().moveable = false;
            GamePlayer.local.gameObject.transform.position = warpPoints[playerIndex].transform.position;
        for (var i = 0; i < playerList.Count; i++)
        {


            if (playerList[i].GetComponent<GamePlayer>().role != Role.Ghost)//also vampire
            {
                lightBeams[i].SetActive(true);
            }
        }

        //}
        // TODO: start timer to talley
        
        
        // TODO: Show UI like Among Us emergency meeting announcement?

        // Show the chat button.
        chatButton.SetActive(true);
    }

    [ClientRpc]
    private void RpcOnMeetingEnd()
    {
        GameLogger.LogClient("The meeting has ended.");

        // TODO: Enable player movement.
        GamePlayer.local.GetComponent<Controller>().moveable = true;
        // Hide the chat button.
        chatButton.SetActive(false);
    }

    [Command(ignoreAuthority = true)]
    public void CmdTallyVote(int indexValue)
    {
        meetingTally[indexValue] += 1;
    }
#endregion
}
