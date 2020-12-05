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
    private void Awake()
    {
        //OnItemDepo.AddListener(CmdDepoItem);
    }
    [Command(ignoreAuthority = true)]
    public void CmdDepoItem(int itemValue)
    {
        GetComponent<GameManager>().AddTaskProgress(itemValue);
    }





}
