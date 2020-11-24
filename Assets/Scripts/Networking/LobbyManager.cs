using Mirror;
using TMPro;

namespace VampireVillage.Network
{
    public class LobbyManager : NetworkBehaviour
    {
        [SyncVar(hook = nameof(UpdateRoom))]
        public Room room;

        public readonly SyncList<ServerPlayer> players = new SyncList<ServerPlayer>();

        public TMP_Text roomCodeText;

        private VampireVillageNetwork network;

        [System.NonSerialized]
        public NetworkManagerMode mode = NetworkManagerMode.Offline;

        public void Awake()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        }

        public override void OnStartServer()
        {
            mode = NetworkManagerMode.ServerOnly;

            // Register the Lobby Manager to the server's Room Manager.
            network.roomManager.RegisterLobbyManager(this, gameObject.scene);
        }

        public override void OnStartClient()
        {
            mode = NetworkManagerMode.ClientOnly;
        }

        public void RegisterRoom(Room room)
        {
            this.room = room;
        }

        public void AddPlayer(ServerPlayer player)
        {
            players.Add(player);
            network.InstantiateLobbyPlayer(gameObject, player);
        }

        public void UpdateRoom(Room oldRoom, Room newRoom)
        {
            roomCodeText.text = newRoom.code;
        }
    }
}
