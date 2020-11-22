using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour
{
    public GameObject InfectButton;
    public GameObject ReportButton;
    public GameObject InteractButton;



    // Start is called before the first frame update
    void Start()
    {
        Player.playerSpawned.AddListener(BeginUI);
    }


    void BeginUI()
    {
        InfectButton.SetActive(false);
        ReportButton.SetActive(false);
        InteractButton.SetActive(false);


        if (Player.local.role == Player.Role.VampireLord)
        {
            InfectButton.SetActive(true);
            Button infectButton = InfectButton.GetComponent<Button>();
            infectButton.onClick.AddListener(Infect);
        }
        else if (Player.local.role == Player.Role.Villager)
        {
            InteractButton.SetActive(true);
            Button interactButton = InteractButton.GetComponent<Button>();
            interactButton.onClick.AddListener(Interact);
        }
        else if (Player.local.role == Player.Role.Infected) // This shouldn't happen in a typical game
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
        Player.local.gameObject.GetComponent<VampireLord>().Infect();
    }

    void Report()
    {
        Player.local.gameObject.GetComponent<Infected>().Report();
    }
    void Interact()
    {
        Player.local.gameObject.GetComponent<Controller>().Interact();
    }
  




}
