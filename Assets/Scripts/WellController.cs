using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellController : Interactable
{
    //public static int totalCompletionProgress;
    public int currentItemCompletion = 0;
    public GameObject playerObject;
    public Item emptyItem;
    public WellManager wellManager;
    void Start()
    {
        //playerObject = GameObject.Find("Player");
        //totalCompletionProgress = 0;
    }

    // Update is called once per frame
    public override void Interact(GameObject player)
    {
        Controller playerController = player.GetComponent<Controller>();
        //currentItemCompletion = playerController.heldItem.completionValue;
        //totalCompletionProgress += playerController.heldItem.completionValue;
        wellManager.CmdDepoItem(playerController.heldItem.completionValue);//will probably eventually send the whole item data, in the case of special item effects
        playerController.heldItem = emptyItem;
        //Debug.Log(totalCompletionProgress);
    }
}
