using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ImposterFountainController : Task
{
    public GameObject BusketOfRock;
    private int Rocknum;
    public bool taskComplete = false;
    public Item slowdownTimer;
    public GameObject playerReference;


    public void Start()
    {
        Rocknum = 0;
    }

    public void OnGUI()
    {
        if (!taskComplete)
        {
            Rocknum += 1;
            if (Rocknum > 5)
            {
                taskComplete = true;
                CompleteTask(gameObject, slowdownTimer);
            }
        }
    }




}
