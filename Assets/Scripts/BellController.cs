using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VampireVillage.Network;

public class BellController : Interactable
{
    private MeetingManager meetingManager;

    public void Start()
    {
        meetingManager = GameManager.local.GetComponent<MeetingManager>();
    }

    public override void Interact(GameObject player)
    {
        Role role = GamePlayer.local.role;
        if (role == Role.Villager || role == Role.Infected)
            meetingManager.CmdStartMeeting();
    }
}
