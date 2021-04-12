using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ImposterFountainController : Task
{
    public GameObject Rock;
    private int Rocknum;
    public bool taskComplete = false;
    public Item slowdownTimer;
    public GameObject playerReference;


    public void Start()
    {
        Rocknum = 0;
        //playerReference = TaskSpawner.currentPlayer();
    }

    public void OnGUI()
    {
        //bool buttonStatus = PourButton.GetComponent<MouseHoldController>().isMouseHeld();
        if (!taskComplete)
        {
            Rocknum += 1;
            if (Rocknum > 5)
            {
                taskComplete = true;
                CompleteTask(gameObject, slowdownTimer);
            }
            //Debug.Log(candleTop);
        }
    }




}
