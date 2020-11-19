using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using VampireVillage.Network;

public class StartMenu : MonoBehaviour
{
    [Header("Server Only")]
    public GameObject serverPanel;

    [Header("Client Only")]
    public TMP_InputField nameInput;
    public GameObject connectingPanel;

    [Header("Online State")]
    public GameObject onlinePanel;
    public Button hostButton;
    public Button joinButton;
    public TMP_InputField roomInput;

    [Header("Offline State")]
    public GameObject offlinePanel;
    public Button reconnectButton;

    private VampireVillageNetwork network;

    private void Awake()
    {
        // Handle network callbacks.
        network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        network.OnNetworkStart += OnNetworkStart;
        network.OnNetworkOnline += OnNetworkOnline;
        network.OnNetworkOffline += OnNetworkOffline;

        // Handle getting and saving name.
        nameInput.text = ApplicationManager.GetPlayerName();
        nameInput.onEndEdit.AddListener(newName => ApplicationManager.SetPlayerName(newName));

        // Handle creating room.
        hostButton.onClick.AddListener(() =>
        {
            GameLogger.LogClient("Creating new room...");
            SetName();
            Client.instance.CmdHostRoom();
        });

        // Handle joining room.
        joinButton.onClick.AddListener(() =>
        {
            string roomCode = roomInput.text;
            SetName();
            GameLogger.LogClient($"Joining room {roomCode}...");
            Client.instance.CmdJoinRoom(roomCode);
        });

        // Handle reconnecting.
        reconnectButton.onClick.AddListener(network.StartClient);

        // Show online/offline menu.
        if (network.isNetworkActive)
            OnNetworkStart();
        else
            OnNetworkOffline();
    }

    private void OnNetworkStart()
    {
        if (network.mode == NetworkManagerMode.ServerOnly)
        {
            onlinePanel.SetActive(false);
            offlinePanel.SetActive(false);
            connectingPanel.SetActive(false);
            nameInput.gameObject.SetActive(false);
            serverPanel.SetActive(true);
        }
        else
        {
            // Show connecting panel.
            offlinePanel.SetActive(false);
            onlinePanel.SetActive(false);
            connectingPanel.SetActive(true);
        }
    }

    private void OnNetworkOnline()
    {
        connectingPanel.SetActive(false);
        offlinePanel.SetActive(false);
        onlinePanel.SetActive(true);
    }

    private void OnNetworkOffline()
    {
        connectingPanel.SetActive(false);
        onlinePanel.SetActive(false);
        offlinePanel.SetActive(true);
    }

    private void SetName()
    {
        if (nameInput.text.Length > 0)
            Client.instance.CmdSetName(nameInput.text);
        else
            Client.instance.CmdSetName("Player");
    }

    private void OnDestroy()
    {
        network.OnNetworkStart -= OnNetworkStart;
        network.OnNetworkOnline -= OnNetworkOnline;
        network.OnNetworkOffline -= OnNetworkOffline;
    }
}
