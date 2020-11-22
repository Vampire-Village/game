using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

/// <summary>
/// TODO:
/// - Change from MonoBehavior -> NetworkBehavior
/// - Get/set the player current role here.
/// </summary>
public class Player : NetworkBehaviour
{
    public static Player local;
    public static UnityEvent playerSpawned = new UnityEvent();

    public enum Role
    {
        PlaceHolder,
        Villager,
        VampireLord,
        Infected
    }
    [SyncVar(hook = nameof(SetRole))]
    public Role role;

    void SetRole(Role oldRole, Role newRole)
    {
        gameObject.GetComponent<Villager>().enabled = false;
        gameObject.GetComponent<VampireLord>().enabled = false;
        gameObject.GetComponent<Infected>().enabled = false;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        
        if (newRole == Role.Villager)
        {
            gameObject.GetComponent<Villager>().enabled = true;
            //local.gameObject.tag = "Villager";
        }
        else if (newRole == Role.VampireLord)
        {
            gameObject.GetComponent<VampireLord>().enabled = true;
            gameObject.GetComponent<SphereCollider>().enabled = true;
            //vampireInfect.radius = 0.5f;
            //vampireInfect.isTrigger = true;
            //local.gameObject.tag = "Vampire";
        }
        else if (newRole == Role.Infected)
        {
            gameObject.GetComponent<Infected>().enabled = true;
            //local.gameObject.tag = "Infected";
        }

    }

    [Command]
    public void CmdSetRole(Role targetRole)
    {
        role = targetRole;
        TargetSetRole();
    }

    [TargetRpc]
    void TargetSetRole()
    {
        playerSpawned.Invoke();
    }

    //public void Awake()
    //{
    //    local = this;
    //}

    // TODO: Switch to this once networking code catches up.
     public override void OnStartAuthority()
     {
        local = this;
        Role randomRole = (Role)Random.Range(1, 4);
        CmdSetRole(randomRole);
        
        

       
        
    }
}
