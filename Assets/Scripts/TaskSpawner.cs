using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSpawner : Interactable
{
    public GameObject taskPrefab;
    public GameObject playerObject;
    
    void Start()
    {
        playerObject = GameObject.Find("Player");
    }

    public override void Interact()
    {
        if (!playerObject.GetComponent<Controller>().inTask)
        {
            playerObject.GetComponent<Controller>().startTask();
            Instantiate(taskPrefab);
        }
    }
}
