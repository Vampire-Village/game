using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GrassController : Task
{

    public GameObject grass1;
    public GameObject grass1sprite;
    public GameObject grass2;
    public GameObject grass2sprite;
    public GameObject grass3;
    public GameObject grass3sprite;
    //public GameObject grass4;

    public int pullAmount = 3;

    private int pull1, pull2, pull3, pull4;
    private int remaining = 3;

    public bool taskComplete = false;
    public Item completeItem;

    public void Start()
    {
        pull1 = pull2 = pull3 = pull4 = pullAmount;
        Button g1 = grass1.GetComponent<Button>();
        Button g2 = grass2.GetComponent<Button>();
        Button g3 = grass3.GetComponent<Button>();
        //Button g4 = grass4.GetComponent<Button>();
        g1.onClick.AddListener(PullGrass1);
        g2.onClick.AddListener(PullGrass2);
        g3.onClick.AddListener(PullGrass3);
        //g4.onClick.AddListener(PullGrass4);
    }

    public void OnGUI()
    {
        if (remaining == 0)
        {
            taskComplete = true;
            CompleteTask(gameObject, completeItem);
            remaining = 3;
        }        
    }

    int PullGrass(int timesPulled)
    {
        return (timesPulled - 1);
    }

    void PullGrass1()
    {
        pull1 = PullGrass(pull1);
        if (pull1 == 0)
        {
            remaining -= 1;
            grass1.SetActive(false);
            grass1sprite.SetActive(false);
        }
    }

    void PullGrass2()
    {
        pull2 = PullGrass(pull2);
        if (pull2 == 0)
        {
            remaining -= 1;
            grass2.SetActive(false);
            grass2sprite.SetActive(false);
        }
    }

    void PullGrass3()
    {
        pull3 = PullGrass(pull3);
        if (pull3 == 0)
        {
            remaining -= 1;
            grass3.SetActive(false);
            grass3sprite.SetActive(false);
        }
    }

    /*
    void PullGrass4()
    {
        pull4 = PullGrass(pull4);
        if (pull4 == 0)
        {
            remaining -= 1;
            grass4.SetActive(false);
        }
    }
    */
}
