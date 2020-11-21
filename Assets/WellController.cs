using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WellController : Interactable
{
    public int totalCompletionProgress;
    public GameObject playerObject;
    void Start()
    {
        //playerObject = GameObject.Find("Player");
        totalCompletionProgress = 0;
    }

    // Update is called once per frame
    public override void Interact(GameObject player)
    {

    }
}
