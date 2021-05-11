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
        Role role = GamePlayer.local.role;
        if (role == Role.Villager || role == Role.Infected)
        {
            playerObject = player;
            Controller playerController = playerObject.GetComponent<Controller>();
            if (!playerController.inTask)
            {
                playerController.StartTask();
                GameObject taskObject;
                taskObject = Instantiate(taskPrefab);
                //taskObject.playerReference = playerObject;
            }
        }
    }
    //public GameObject currentPlayer()
    //{
        
            //return playerObject;
        
    //}
}
