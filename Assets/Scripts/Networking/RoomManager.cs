using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace VampireVillage.Network
{
    /// <summary>
    /// On the server: Manages the room creation and joining.
    /// On the client: Holds information about the joined room.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public uint codeLength = 4;
        public readonly Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        private readonly List<Scene> test = new List<Scene>();

        [System.NonSerialized]
        public NetworkManagerMode mode = NetworkManagerMode.Offline;

        public Room CreateRoom()
        {
            // Generate a unique room code.
            string roomCode;
            do
            {
                roomCode = Room.GenerateCode(codeLength);
            } while (rooms.ContainsKey(roomCode));

            // Create the room and store it.
            Room room = new Room(roomCode);
            rooms.Add(roomCode, room);

            return room;
        }

        public Room JoinRoom(string roomCode)
        {
            // Return null if room doesn't exist.
            // TODO: Proper error handling.
            if (!rooms.ContainsKey(roomCode))
                return null;

            // Get the room information.
            // TODO: Check if room hasn't started game.
            Room room = rooms[roomCode];
            return room;
        }

        public void RegisterLobbyManager(LobbyManager lobbyManager, Scene lobbyScene)
        {
            StartCoroutine(RegisterLobbyManagerAsync(lobbyManager, lobbyScene));
        }

        private IEnumerator RegisterLobbyManagerAsync(LobbyManager lobbyManager, Scene lobbyScene)
        {
            // NOTE: This is just here to fix race condition between RegisterLobbyManager and setting room.lobbyScene.
            // TODO: Better scene management.
            yield return new WaitForSeconds(3);

            // Search for the rooms scene.
            foreach (var room in rooms.Values)
            {
                if (room.lobbyScene == lobbyScene)
                {
                    room.lobbyManager = lobbyManager;
                    lobbyManager.RegisterRoom(room);
                    room.isRoomInitialized = true;
                    break;
                }
            }
        }
    }
}
