using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VampireVillage.Network
{
    /// <summary>
    /// Manages the room creation and states on the server.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
#region Properties
#region Client & Server Properties
        /// <summary>
        /// Length of the room code.
        /// </summary>
        public uint codeLength = 4;
#endregion

#region Server-only Properties
#if UNITY_SERVER || UNITY_EDITOR
        /// <summary>
        /// Minimum players to start a game.
        /// </summary>
        public uint minPlayers = 4;

        /// <summary>
        /// Maximum players in a room.
        /// </summary>
        public uint maxPlayers = 10;

        private readonly Dictionary<string, Room> rooms = new Dictionary<string, Room>();

#if UNITY_EDITOR
        [System.NonSerialized]
        public NetworkMode mode = NetworkMode.Offline;

        public Dictionary<string, Room> editorRooms { get { return rooms; } }
#endif
#endif
#endregion
#endregion

#region Server Methods
#if UNITY_SERVER || UNITY_EDITOR
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

        public Room GetRoom(string roomCode)
        {
            // Return null if room doesn't exist.
            // TODO: Proper error handling.
            if (!rooms.ContainsKey(roomCode))
                return null;

            Room room = rooms[roomCode];
            return room;
        }

        public void JoinRoom(Room room, ServerPlayer player)
        {
            room.players.Add(player);
            if (room.state == RoomState.Lobby)
                room.lobbyManager.AddPlayer(player);
        }

        public Room LeaveRoom(ServerPlayer player)
        {
            // Remove the player from the room.
            Room room = player.room;
            player.room = null;
            room.players.Remove(player);

            // Remove the player from the manger.
            if (room.state == RoomState.Lobby)
                room.lobbyManager.RemovePlayer(player);
            else if (room.state == RoomState.Game)
                room.gameManager.RemovePlayer(player);

            // Change the room host if player was the host.
            if (room.host == player && room.players.Count > 0)
            {
                room.host = room.players[0];
                room.lobbyManager.SetHost(room.host);
                GameLogger.LogServer($"Room {room.code} changed its host.\nNew host:{room.host.id}\nOld host:{player.id}");
            }

            return room;
        }

        public void RemoveRoom(Room room)
        {
            rooms.Remove(room.code);
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
                if (room.state == RoomState.Lobby && room.scene == lobbyScene)
                {
                    room.lobbyManager = lobbyManager;
                    lobbyManager.RegisterRoom(room);
                    room.isLobbyInitialized = true;
                    break;
                }
            }
        }

        public void RegisterGameManager(GameManager gameManager, Scene gameScene)
        {
            StartCoroutine(RegisterGameManagerAsync(gameManager, gameScene));
        }

        private IEnumerator RegisterGameManagerAsync(GameManager gameManager, Scene gameScene)
        {
            // NOTE: This is just here to fix race condition between RegisterLobbyManager and setting room.lobbyScene.
            // TODO: Better scene management.
            yield return new WaitForSeconds(3);

            // Search for the rooms scene.
            foreach (var room in rooms.Values)
            {
                if (room.state == RoomState.Game && room.scene == gameScene)
                {
                    room.gameManager = gameManager;
                    gameManager.RegisterRoom(room);
                    room.isGameInitialized = true;
                    break;
                }
            }
        }
#endif
#endregion
    }
}
