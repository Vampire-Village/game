using UnityEngine;
using UnityEngine.Events;
using VampireVillage.Network;
using Mirror;

public class GamePlayer : BasePlayer
{
#region Properties
    public new static GamePlayer local;

    public new static UnityEvent OnPlayerSpawned = new UnityEvent();
    public static UnityEvent OnRoleUpdated = new UnityEvent();
    
    [SyncVar(hook = nameof(SetRole))]
    public Role role = Role.None;

    public Controller controller { get; private set; }
    public Villager villager { get; private set; }
    public VampireLord vampireLord { get; private set; }
    public Infected infected { get; private set; }

    private SphereCollider sphereCollider;

    private GameManager gameManager;
#endregion

#region Unity Methods
    private void Awake()
    {
        // Get component references.
        controller = GetComponent<Controller>();
        villager = GetComponent<Villager>();
        vampireLord = GetComponent<VampireLord>();
        infected = GetComponent<Infected>();
        sphereCollider = GetComponent<SphereCollider>();
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
        // Update the role.
        Role oldRole = role;
        role = targetRole;

        // Report to the game manager to switch team.
        if (oldRole == Role.Villager && targetRole == Role.Infected)
        {
            gameManager.UpdatePlayerTeam(this, oldRole, targetRole);
        }

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

        // // TEST: Randomize role.
        // Role randomRole = (Role)UnityEngine.Random.Range(1, 4);
        // CmdSetRole(randomRole);
    }

    void SetRole(Role oldRole, Role newRole)
    {
        // Disable all role components.
        villager.enabled = false;
        vampireLord.enabled = false;
        infected.enabled = false;
        sphereCollider.enabled = false;

        switch (newRole)
        {
            case Role.Villager:
                villager.enabled = true;
                break;
            case Role.VampireLord:
                vampireLord.enabled = true;
                sphereCollider.enabled = true;
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
            OnRoleUpdated?.Invoke();
            GameLogger.LogClient($"Player is now {role.ToString()}!");
        }
    }
#endregion
}
