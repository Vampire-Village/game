using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// TODO:
/// - Change from MonoBehavior -> NetworkBehavior
/// - Get/set the player current role here.
/// </summary>
public class Player : MonoBehaviour
{
    public static Player local;

    public void Awake()
    {
        local = this;
    }

    // TODO: Switch to this once networking code catches up.
    // public override void OnStartAuthority()
    // {
    //     local = this;
    // }
}
