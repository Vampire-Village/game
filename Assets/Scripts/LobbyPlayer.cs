using UnityEngine.Events;

public class LobbyPlayer : BasePlayer
{
    public new static LobbyPlayer local;

    public new static UnityEvent OnPlayerSpawned = new UnityEvent();

    public override void OnStartAuthority()
    {
        // Set base player.
        base.OnStartAuthority();

        // Set lobby player.
        local = this;
        OnPlayerSpawned.Invoke();
    }
}
