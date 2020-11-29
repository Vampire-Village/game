using UnityEngine;
using UnityEngine.UI;
using VampireVillage.Network;
using TMPro;

/// <summary>
/// Handles the UI logic for the StartMenu scene.
/// </summary>
public class StartMenu : MonoBehaviour
{
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

    [Header("Popup")]
    public GameObject popupPanel;
    public TMP_Text popupText;
    public Button popupButton;

    private VampireVillageNetwork network;
    private uint codeLength;

    private void Awake()
    {
        // Get references.
        network = VampireVillageNetwork.singleton as VampireVillageNetwork;
        codeLength = network.roomManager.codeLength;

        // Show menu depending on current network state.
        if (network.isNetworkActive)
        {
            if (network.isNetworkConnected)
                OnNetworkOnline();
            else
                OnNetworkStart();
        }
        else
            OnNetworkOffline();
            
        // Handle network callbacks.
        network.OnNetworkStart += OnNetworkStart;
        network.OnNetworkOnline += OnNetworkOnline;
        network.OnNetworkOffline += OnNetworkOffline;

        // Set name input to last used name.
        nameInput.text = ApplicationManager.GetPlayerName();

        // Add inputs & buttons listeners.
        nameInput.onEndEdit.AddListener(OnEditName);
        roomInput.onValidateInput += OnValidateRoomCode;
        hostButton.onClick.AddListener(HostRoom);
        joinButton.onClick.AddListener(JoinRoom);
        reconnectButton.onClick.AddListener(network.StartClient);
        popupButton.onClick.AddListener(() => popupPanel.SetActive(false));
    }

    private void OnNetworkStart()
    {
        offlinePanel.SetActive(false);
        onlinePanel.SetActive(false);
        connectingPanel.SetActive(true);
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

    private void OnEditName(string newName)
    {
        // TODO: Make sure name is valid and/or not offensive.
        ApplicationManager.SetPlayerName(newName);
    }

    private char OnValidateRoomCode(string input, int charIndex, char addedChar)
    {
        // Make sure the room code is valid length.
        if (charIndex + 1 > codeLength)
            return '\0';

        // Make sure the user enters a letter & convert to uppercase if necesssary.
        if (char.IsLetter(addedChar))
        {
            if (char.IsLower(addedChar))
                addedChar = char.ToUpper(addedChar);
            return addedChar;
        }
        else
            return '\0';
    }

    private void HostRoom()
    {
        SetInputsAndButtonsActive(false);
        SetName();
        Client.local.HostRoom(OnHostRoomError);
    }

    private void OnHostRoomError(string errorMessage)
    {
        ShowPopup(errorMessage);
        SetInputsAndButtonsActive(true);
    }

    private void JoinRoom()
    {
        string roomCode = roomInput.text;
        SetInputsAndButtonsActive(false);
        SetName();
        Client.local.JoinRoom(roomCode, OnJoinRoomError);
    }

    private void OnJoinRoomError(string errorMessage)
    {
        ShowPopup(errorMessage);
        SetInputsAndButtonsActive(true);
    }

    private void SetName()
    {
        Client.local.CmdSetName(nameInput.text);
    }

    private void SetInputsAndButtonsActive(bool isActive)
    {
        nameInput.interactable = isActive;
        roomInput.interactable = isActive;
        hostButton.interactable = isActive;
        joinButton.interactable = isActive;
    }

    private void ShowPopup(string message)
    {
        popupText.text = message;
        popupPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        // Remove network listeners.
        network.OnNetworkStart -= OnNetworkStart;
        network.OnNetworkOnline -= OnNetworkOnline;
        network.OnNetworkOffline -= OnNetworkOffline;

        // Remove other listeners.
        nameInput.onEndEdit.RemoveAllListeners();
        roomInput.onValidateInput -= OnValidateRoomCode;
        hostButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
        reconnectButton.onClick.RemoveAllListeners();
        popupButton.onClick.RemoveAllListeners();
    }
}
