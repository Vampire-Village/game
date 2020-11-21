using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSpawner : Interactable
{
    public GameObject taskPrefab;
    public GameObject playerObject;
    
    void Start()
    {
        //playerObject = GameObject.Find("Player");
    }

    public override void Interact(GameObject player)
    {
        playerObject = player;
        if (!playerObject.GetComponent<Controller>().inTask)
        {
            playerObject.GetComponent<Controller>().startTask();
            GameObject taskObject;
            taskObject = Instantiate(taskPrefab);
            //taskObject.playerReference = playerObject;
        }
    }
    public GameObject currentPlayer()
    {
        
            return playerObject;
        
    }
}
