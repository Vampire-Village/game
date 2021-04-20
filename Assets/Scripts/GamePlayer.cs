using UnityEngine;
using UnityEngine.Events;
using VampireVillage.Network;
using Mirror;

public class GamePlayer : BasePlayer
{
#region Properties
    public new static GamePlayer local;

    public new static UnityEvent OnPlayerSpawned = new UnityEvent();
    public static UnityEvent OnLocalRoleUpdated = new UnityEvent();
    
    [SyncVar(hook = nameof(SetRole))]
    public Role role = Role.None;

    public UnityEvent OnRoleUpdated = new UnityEvent();

    public Controller controller { get; private set; }
    public Villager villager { get; private set; }
    public VampireLord vampireLord { get; private set; }
    public Infected infected { get; private set; }

    public Ghost ghost { get; private set; }

    private SphereCollider sphereCollider;

    private GameManager gameManager;

    private BloodHunt bloodHuntScript;
#endregion

#region Unity Methods
    private void Awake()
    {
        // Get component references.
        controller = GetComponent<Controller>();
        villager = GetComponent<Villager>();
        vampireLord = GetComponent<VampireLord>();
        infected = GetComponent<Infected>();
        ghost = GetComponent<Ghost>();
        sphereCollider = GetComponent<SphereCollider>();
        bloodHuntScript = GetComponent<BloodHunt>();
    }
#endregion

#region Server Methods
    public void RegisterGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    [Command(ignoreAuthority = true)]
    public void CmdSetRole(Role targetRole)
    {
#if UNITY_SERVER || UNITY_EDITOR
        // Update the role.
        Role oldRole = role;
        role = targetRole;

        // Report to the game manager to switch team.
        if (oldRole == Role.Villager && targetRole == Role.Infected)
        {
            gameManager.UpdatePlayerTeam(this, oldRole, targetRole);
        }
#endif
    }

    
#endregion

#region Client Methods
    public override void OnStartAuthority()
    {
        // Set base player.
        base.OnStartAuthority();

        // Set game player.
        local = this;
        OnPlayerSpawned.Invoke();
    }

    void SetRole(Role oldRole, Role newRole)
    {
        // Disable all role components.
        villager.enabled = false;
        vampireLord.enabled = false;
        infected.enabled = false;

        switch (newRole)
        {
            case Role.Villager:
                villager.enabled = true;
                break;
            case Role.VampireLord:
                vampireLord.enabled = true;
                //bloodHuntScript.enabled = true;
                break;
            case Role.Infected:
                infected.enabled = true;
                break;
            default:
                GameLogger.LogClient("Role is none. Something is wrong.");
                return;
        }

        if (hasAuthority)
        {
            OnLocalRoleUpdated?.Invoke();
            GameLogger.LogClient($"Player is now {newRole.ToString()}!");
        }
        OnRoleUpdated?.Invoke();
    }
#endregion
}
