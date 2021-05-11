using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infected : MonoBehaviour
{

    public GameObject pingSprite;

    private float xPos;
    private float zPos;

    private GameObject minimap;

    //private int cooldown = 5;
    private int cdSec = 0;

    private PlayerUI playerUI;

    private float pingHeight = 14;
    private float pingZoffset = 1.1f;


    private void Start()
    {
        pingSprite = Resources.Load("Ping") as GameObject;
        gameManager = GameManager.local;
    }
    public void RegisterUI(PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }

    public void Ping()
    {
        Debug.Log("*ping*");
        xPos = GetComponent<Transform>().position.x;
        zPos = GetComponent<Transform>().position.z;

        CmdPing(pingSprite, xPos, zPos);
    }

    [Command]
    private void CmdPing(GameObject pingSprite, float xPos,float zPos)
    {
        GameObject ping = Instantiate(pingSprite, new Vector3(xPos, pingHeight, zPos + pingZoffset), Quaternion.Euler(90, 0, 0)) as GameObject;
        Debug.Log(ping);
        gameManager.Ping(ping);
    }

}
