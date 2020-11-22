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
        Villager,
        VampireLord,
        Infected
    }

    public Role role;

    //public void Awake()
    //{
    //    local = this;
    //}

    // TODO: Switch to this once networking code catches up.
     public override void OnStartAuthority()
     {
        role = (Role)Random.Range(0, 3);
        local = this;
        if(local.role == Role.Villager)
        {
            local.gameObject.AddComponent<Villager>();
            local.gameObject.tag = "Villager";
        }
        else if(local.role == Role.VampireLord)
        {
            local.gameObject.AddComponent<VampireLord>();
            SphereCollider vampireInfect = local.gameObject.AddComponent<SphereCollider>();
            vampireInfect.radius = 0.5f;
            vampireInfect.isTrigger = true;
            local.gameObject.tag = "Vampire";
        }
        else if(local.role == Role.Infected)
        {
            local.gameObject.AddComponent<Infected>();
            local.gameObject.tag = "Infected";
        }
        playerSpawned.Invoke();
        
    }
}
