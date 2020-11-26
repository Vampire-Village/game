using UnityEngine.Events;
using Mirror;
using TMPro;

public abstract class BasePlayer : NetworkBehaviour
{
    public static BasePlayer local;

    public static UnityEvent OnPlayerSpawned = new UnityEvent();

    [SyncVar(hook = nameof(SetName))]
    public string playerName;

    public TMP_Text nameText;

    public override void OnStartAuthority()
    {
        local = this;
        OnPlayerSpawned?.Invoke();
    }

    public void SetName(string oldName, string newName)
    {
        nameText.text = newName;
    }
}
