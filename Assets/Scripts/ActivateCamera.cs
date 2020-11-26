using UnityEngine;
using Cinemachine;

public class ActivateCamera : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        BasePlayer.OnPlayerSpawned.AddListener(FollowPlayer);
    }

    public void FollowPlayer()
    {
        vcam.LookAt = BasePlayer.local.transform;
        vcam.Follow = BasePlayer.local.transform;
    }

    private void OnDestroy()
    {
        BasePlayer.OnPlayerSpawned.RemoveListener(FollowPlayer);
    }
}
