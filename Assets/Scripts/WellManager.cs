using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using VampireVillage.Network;

public class WellManager : NetworkBehaviour
{

    [SyncVar(hook = nameof(UpdateProgress))]
    public int totalWellProgress = 0;

    public Slider taskProgress;

    private int maximumProgress = 100;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    [Command(ignoreAuthority = true)]
    public void CmdDepoItem(int itemValue)
    {
#if UNITY_SERVER || UNITY_EDITOR
        totalWellProgress += itemValue;
        if (totalWellProgress >= maximumProgress)
        {
            gameManager.GameOver(Team.Villagers);
        }
#endif
    }

    private void UpdateProgress(int oldValue, int newValue)
    {
        taskProgress.value = (float)(newValue) / (float)(maximumProgress);
    }
}
