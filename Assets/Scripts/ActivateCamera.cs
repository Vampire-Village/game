using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
public class ActivateCamera : MonoBehaviour
{
    void Start()
    {
        Player.playerSpawned.AddListener(FollowPlayer);
    }

    void FollowPlayer()
    {
        CinemachineVirtualCamera vcam = gameObject.GetComponent<CinemachineVirtualCamera>();
        vcam.LookAt = Player.local.GetComponent<Transform>();
        vcam.Follow = Player.local.GetComponent<Transform>();
    }
}
