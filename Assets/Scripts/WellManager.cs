using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using VampireVillage.Network;

public class WellManager : NetworkBehaviour
{
    //public static WellManager progress;
    //public new UnityEvent OnItemDepo = new UnityEvent();
    public GameManager gameManager;
    [SyncVar]
    public int totalWellProgress = 0;
    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        //OnItemDepo.AddListener(CmdDepoItem);
    }
    [Command(ignoreAuthority = true)]
    public void CmdDepoItem(int itemValue)
    {
        totalWellProgress += itemValue;
        if (totalWellProgress >= 100)
        {
            gameManager.GameOver(Team.Villagers);
        }

    }





}
