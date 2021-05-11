using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using VampireVillage.Network;
using TMPro;

public class Infected : NetworkBehaviour
{
    public GameObject pingSprite;
    private float xPos;
    private float zPos;
    private GameObject minimap;

    private PlayerUI playerUI;

    private float pingHeight = 20;
    private float pingZoffset = 1.1f;

    public void RegisterUI(PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }

    public void Ping()
    {
        
        Debug.Log("*ping*");
        //minimap = GameObject.Find("/UI Canvas/VampireMinimap"); // move to start when testing actual game
        xPos = GetComponent<Transform>().position.x;
        zPos = GetComponent<Transform>().position.z;
        GameObject ping = Instantiate(pingSprite, new Vector3(xPos, pingHeight, zPos + pingZoffset), Quaternion.Euler(90, 0, 0)) as GameObject;
        //ping.transform.parent = minimap.transform;
        CmdPing(ping);
    }

    [Command]
    private void CmdPing(GameObject ping)
    {
        GameManager.local.Ping(ping);
    }
}
