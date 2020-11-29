using UnityEngine;
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

    [TargetRpc]
    public void TargetSetPosition(Vector3 position)
    {
        // TODO: This is just a silly solution to solve client vs server authority issue.
        // Need better fix in the future.
        transform.position = position;
    }

    public void SetName(string oldName, string newName)
    {
        // Set gameobject name for the editor.
        name = $"Player ({newName})";

        // Set the floating name text.
        nameText.text = newName;
    }
}
