using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using VampireVillage.Network;

public class Infected : NetworkBehaviour
{
    public GameObject pingSprite;
    private float xPos;
    private float zPos;

    private PlayerUI playerUI;

    private float pingHeight = 15;
    private float pingZoffset = 1.1f;
 
    private GameManager gameManager = null;

    public void RegisterUI(PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }

    public void RegisterGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void Ping()
    {
        xPos = GetComponent<Transform>().position.x;
        zPos = GetComponent<Transform>().position.z;
        CmdPing(xPos, pingHeight, zPos, pingZoffset);
    }

    [Command]
    private void CmdPing(float xPos, float pingHeight, float zPos, float pingZoffset)
    {
#if UNITY_SERVER || UNITY_EDITOR
        gameManager.Ping(xPos, pingHeight, zPos, pingZoffset);
#endif
    }
}
