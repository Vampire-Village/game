using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireVillage.Network;

public class BellController : Interactable
{
    // Start is called before the first frame update
    //private MeetingManager meetingManager;
    public void Start()
    {
        //meetingManager = ;
    }
    public override void Interact(GameObject player)
    {
        Role role = GamePlayer.local.role;
        if (role == Role.Villager || role == Role.Infected)
        GameManager.local.GetComponent<MeetingManager>().StartMeeting();
    }
}
