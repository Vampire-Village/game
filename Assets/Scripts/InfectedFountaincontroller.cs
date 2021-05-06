using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InfectedFountaincontroller : Task
{
    public GameObject Rock1;
    public GameObject Rock2;
    public GameObject Rock3;
    public GameObject Rock4;
    public GameObject Rock5;
    public GameObject Rock6;
    private int Rocknum;
    public bool taskComplete = false;
    public Item slowdownTimer;


    public void Start()
    {
        Rocknum = 0;
    }

    public void updatestat(int tke)
    {
        Rocknum = tke;
        Debug.Log(Rocknum.ToString());
        if (Rocknum == 6)
        {
            Debug.Log("finish");
            taskComplete = true;
            CompleteTask(gameObject, slowdownTimer);
            Rocknum = 0;
        }
    }



}
