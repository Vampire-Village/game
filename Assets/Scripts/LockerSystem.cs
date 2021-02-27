using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using VampireVillage.Network;

public class LockerSystem : NetworkBehaviour
{
#region Properties
    public int maxPlayers = 2;
    public Transform lockerPoint;
    public Transform outPoint;

    private readonly List<ServerPlayer> players = new List<ServerPlayer>();

    private Door door;

    [SerializeField]
    private GameManager gameManager;

    private VampireVillageNetwork network;

    private enum LockerSystemStatus
    {
        GotIn,
        GotOut,
        FullHouse
    }
#endregion

#region Unity Methods
    private void Start()
    {
        network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        door = GetComponent<Door>();
    }
#endregion

#region Server Methods
    [Command(ignoreAuthority=true)]
    private void CmdActivateLocker(NetworkConnectionToClient conn = null)
    {
        // Check if player is inside.
        ServerPlayer player = network.GetPlayer(conn);
        if (players.Contains(player))
            GetOut(conn, player);
        else
            GetIn(conn, player);
    }

    private void GetIn(NetworkConnectionToClient conn, ServerPlayer player)
    {
        // Check if house is not full.
        if (players.Count >= maxPlayers)
        {
            TargetActivatedLocker(conn, LockerSystemStatus.FullHouse);
            return;
        }

        // Otherwise, add player to the locker.
        players.Add(player);
        TargetActivatedLocker(conn, LockerSystemStatus.GotIn);
    }

    private void GetOut(NetworkConnectionToClient conn, ServerPlayer player)
    {
        // Kick out all the players from the house.
        foreach (var lockerPlayer in players)
            TargetActivatedLocker(lockerPlayer.clientConnection, LockerSystemStatus.GotOut);
        players.RemoveAll(_ => true);
    }
#endregion

#region Client Methods
    public void ActivateLocker()
    {
        GameLogger.LogClient("Activating locker!");
        CmdActivateLocker();
    }

    [TargetRpc]
    private void TargetActivatedLocker(NetworkConnection target, LockerSystemStatus status)
    {
        switch (status)
        {
            case LockerSystemStatus.GotIn:
                GotIn();
                break;
            case LockerSystemStatus.GotOut:
                GotOut();
                break;
            case LockerSystemStatus.FullHouse:
                FullHouse();
                break;
        }
    }

    private void GotIn()
    {
        GameLogger.LogClient("Got in to the house.");
        // TODO: Make this server side one day.
        GamePlayer.local.transform.position = lockerPoint.position;
        GamePlayer.local.controller.moveable = false;
        door.OpenCloseDoor();
    }

    private void GotOut()
    {
        GameLogger.LogClient("Got out of the house.");
        // TODO: Make this server side one day.
        GamePlayer.local.transform.position = outPoint.position;
        GamePlayer.local.controller.moveable = true;
        door.OpenCloseDoor();
    }

    private void FullHouse()
    {
        GameLogger.LogClient("House is full!");
        gameManager.Announce("This house is full!");
    }
#endregion
}
