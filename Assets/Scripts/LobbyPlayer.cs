using Mirror;
using TMPro;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetName))]
    public string playerName;

    public TMP_Text nameText;

    public void SetName(string oldName, string newName)
    {
        nameText.text = newName;
    }
}
