using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellController : Interactable
{
    public static int totalCompletionProgress;
    public GameObject playerObject;
    public Item emptyItem;
    public WellManager wellManager;
    void Start()
    {
        //playerObject = GameObject.Find("Player");
        totalCompletionProgress = 0;
    }

    // Update is called once per frame
    public override void Interact(GameObject player)
    {
        Controller playerController = player.GetComponent<Controller>();
        totalCompletionProgress += playerController.heldItem.completionValue;
        playerController.heldItem = emptyItem;
        Debug.Log(totalCompletionProgress);
    }
}
