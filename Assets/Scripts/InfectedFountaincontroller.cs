using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InfectedFountaincontroller : Task
{
    public GameObject Busket;
    public GameObject Rocks;
    private int Rocknum;
    public bool taskComplete = false;
    public Item slowdownTimer;


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
