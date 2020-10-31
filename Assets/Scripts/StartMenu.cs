using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public TMP_InputField nameInput;
    public TMP_InputField roomInput;
    
    void Start()
    {
        // Handle getting and saving name.
        nameInput.text = ApplicationManager.GetPlayerName();
        nameInput.onEndEdit.AddListener((newName) =>
        {
            ApplicationManager.SetPlayerName(newName);
        });
    }
}
