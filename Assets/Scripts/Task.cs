using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
    public void CompleteTask(GameObject taskCanvas, Item completeItem)
    {
        //GameObject player = currentPlayer();
        //Controller playerController = player.GetComponent<Controller>();
        //Controller player = GameObject.Find("Player").GetComponent<Controller>();
        Controller player = GamePlayer.local.GetComponent<Controller>();
        player.EndTask(completeItem);
        Debug.Log("Task ended!");
        Destroy(taskCanvas, 2.0f);
    }
}
