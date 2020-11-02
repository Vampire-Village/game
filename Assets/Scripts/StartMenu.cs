using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        network = VampireVillageNetwork.instance;
        network.OnNetworkStart += OnNetworkStart;
    }

    private void OnNetworkStart(bool isServer)
    {
        if (isServer)
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

            // Handle getting and saving name.
            nameInput.text = ApplicationManager.GetPlayerName();
            nameInput.onEndEdit.AddListener((newName) =>
            {
                ApplicationManager.SetPlayerName(newName);
            });

            // Handle reconnecting.
            reconnectButton.onClick.AddListener(() => network.StartClient());

            network.OnNetworkOnline += OnNetworkOnline;
            network.OnNetworkOffline += OnNetworkOffline;
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

    private void OnDestroy()
    {
        network.OnNetworkStart -= OnNetworkStart;
        network.OnNetworkOnline -= OnNetworkOnline;
        network.OnNetworkOffline -= OnNetworkOffline;
    }
}
