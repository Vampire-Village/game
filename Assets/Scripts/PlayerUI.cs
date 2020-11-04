using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    public GameObject InfectButton;
    public GameObject ReportButton;



    // Start is called before the first frame update
    void Start()
    {
        InfectButton.SetActive(false);
        ReportButton.SetActive(false);


        if (gameObject.tag == "Vampire")
        {
            InfectButton.SetActive(true);
            Button infectButton = InfectButton.GetComponent<Button>();
            infectButton.onClick.AddListener(Infect);
        }
        else if (gameObject.tag == "Villager")
        {
        }
        else if (gameObject.tag == "Infected") // This shouldn't happen in a typical game
        {
            ReportButton.SetActive(true);
            Button reportButton = ReportButton.GetComponent<Button>();
            reportButton.onClick.AddListener(Report);
        }
        else
        {
            Debug.Log("Player tag is not assigned");
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    /* Note: Right now, both players are sharing the same UI; later will need to make
    sure that each player has their own UI and buttons are being updated accordingly */










    void Infect()
    {
        gameObject.GetComponent<VampireLord>().Infect();
    }

    void Report()
    {
        gameObject.GetComponent<Infected>().Report();
    }
  




}
