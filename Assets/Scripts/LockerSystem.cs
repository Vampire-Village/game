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

    private readonly List<ServerPlayer> players = new List<ServerPlayer>();

    private Door door;

    [SerializeField]
    private GameManager gameManager;

    private VampireVillageNetwork network;
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
    public void CmdGetIn(NetworkConnectionToClient conn)
    {
        ServerPlayer player = network.GetPlayer(conn);

        // Check if house is not full.
        if (players.Count >= maxPlayers)
        {
            gameManager.Announce("This house is full!");
            // TODO: Play locked door sound here.
            return;
        }

        players.Add(player);
    }

    [Command(ignoreAuthority=true)]
    public void CmdGetOut(NetworkConnectionToClient conn)
    {
        ServerPlayer player = network.GetPlayer(conn);

        players.RemoveAll(_ => true);
        
        // TODO: Actually remove all players from the house.
    }
#endregion
}
