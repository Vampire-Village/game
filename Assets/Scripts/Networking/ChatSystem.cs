using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace VampireVillage.Network
{
    public class ChatSystem : NetworkBehaviour
    {
#region Properties
        public GameObject chatPanel;
        public GameObject chatContent;
        public TMP_InputField chatInput;
        public Button sendButton;
        public GameObject chatBubblePrefab;

        private readonly List<ServerPlayer> players = new List<ServerPlayer>();

        private VampireVillageNetwork network;
#endregion

#region Server Methods
#if UNITY_SERVER || UNITY_EDITOR
        public override void OnStartServer()
        {
            network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        }

        public void AddPlayer(ServerPlayer player)
        {
            players.Add(player);
        }

        public void RemovePlayer(ServerPlayer player)
        {
            players.Remove(player);
        }
#endif

        [Command(ignoreAuthority = true)]
        private void CmdSendChat(string message, NetworkConnectionToClient sender = null)
        {
#if UNITY_SERVER || UNITY_EDITOR
            // Get the sender.
            ServerPlayer senderPlayer = network.GetPlayer(sender);

            // Construct chat.
            Chat chat = new Chat
            {
                sender = senderPlayer.client.playerName,
                message = message
            };

            // Send chat to the players.
            foreach (var player in players)
            {
                TargetReceiveChat(player.clientConnection, chat);
            }
#endif
        }
#endregion

#region Client Methods
        public override void OnStartClient()
        {
            sendButton.onClick.AddListener(SendChat);
        }

        public void ToggleChat()
        {
            chatPanel.SetActive(!chatPanel.activeSelf);
        }

        public void SendChat()
        {
            string message = chatInput.text;
            if (message.Length == 0) return;
            CmdSendChat(message);
            chatInput.text = "";
        }

        [TargetRpc]
        public void TargetReceiveChat(NetworkConnection target, Chat chat)
        {
            // Add chat to the UI.
            GameObject chatBubbleInstance = Instantiate(chatBubblePrefab, chatContent.transform);
            ChatBubble chatBubble = chatBubbleInstance.GetComponent<ChatBubble>();
            chatBubble.SetChat(chat);
        }

        public override void OnStopClient()
        {
            sendButton.onClick.RemoveListener(SendChat);
        }
#endregion
    }
}
