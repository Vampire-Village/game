using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using VampireVillage.Network;
using TMPro;

public class LockerSystem : NetworkBehaviour
{
#region Properties
    public int maxPlayers = 2;
    public Transform lockerPoint;
    public Transform outPoint;
    public GameObject lockerCanvas;
    public TMP_Text nameText1;
    public TMP_Text nameText2;

    private readonly List<ServerPlayer> players = new List<ServerPlayer>();
    public readonly SyncList<string> playerNames = new SyncList<string>();

    private Door door;

    [SerializeField]
    private GameManager gameManager = null;

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
        lockerCanvas.SetActive(false);
        nameText1.text = "";
        nameText2.text = "";
        playerNames.Callback += OnPlayerNamesUpdated;
    }
#endregion

#region Server Methods
    [Command(ignoreAuthority=true)]
    private void CmdActivateLocker(NetworkConnectionToClient conn = null)
    {
#if UNITY_SERVER || UNITY_EDITOR
        // Check if player is inside.
        ServerPlayer player = network.GetPlayer(conn);
        if (players.Contains(player))
            GetOut(conn, player);
        else
            GetIn(conn, player);
#endif
    }

#if UNITY_SERVER || UNITY_EDITOR
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
        playerNames.Add(player.client.playerName);
        TargetActivatedLocker(conn, LockerSystemStatus.GotIn);
    }

    private void GetOut(NetworkConnectionToClient conn, ServerPlayer player)
    {
        // Kick out all the players from the house.
        foreach (var lockerPlayer in players)
            TargetActivatedLocker(lockerPlayer.clientConnection, LockerSystemStatus.GotOut);
        players.RemoveAll(_ => true);
        playerNames.RemoveAll(_ => true);
    }
#endif
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

        // Show canvas.
        lockerCanvas.SetActive(true);
    }

    private void GotOut()
    {
        GameLogger.LogClient("Got out of the house.");

        // Hide canvas.
        lockerCanvas.SetActive(false);

        // TODO: Make this server side one day.
        GamePlayer.local.transform.position = outPoint.position;
        GamePlayer.local.controller.moveable = true;
        door.OpenCloseDoor();
    }

    private void FullHouse()
    {
        GameLogger.LogClient("House is full!");
        if (gameManager != null)
            gameManager.Announce("This house is full!");
    }

    private void OnPlayerNamesUpdated(SyncList<string>.Operation op, int index, string oldItem, string newItem)
    {
        if (playerNames.Count > 0)
        {
            nameText1.text = playerNames[0];
            nameText2.text = playerNames.Count > 1 ? playerNames[1] : "";
        }
        else
        {
            nameText1.text = "";
            nameText2.text = "";
        }
    }

    private void OnDestroy()
    {
        playerNames.Callback -= OnPlayerNamesUpdated;
    }
#endregion
}
