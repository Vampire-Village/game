using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGraves : Interactable
{
    public GameObject teleportTo;
    public int secondsToTeleport = 1;
    public TeleportAnimation teleportAnimation;
    private float graveX;
    private float graveZ;

    public override void Interact(GameObject player)
    {
        StartCoroutine(WaitToTeleport(player));
    }

    // Start is called before the first frame update
    void Start()
    {
        graveX = teleportTo.GetComponent<Transform>().position.x;
        graveZ = teleportTo.GetComponent<Transform>().position.z;
        teleportAnimation = GetComponentInChildren<TeleportAnimation>();
    }

    IEnumerator WaitToTeleport(GameObject player)
    {
        Controller controller = player.GetComponent<Controller>();
        controller.inTask = true;
        teleportAnimation.PlayAnimations();
        yield return new WaitForSeconds(secondsToTeleport);
        float playerY = controller.transform.position.y;
        controller.transform.position = new Vector3(graveX, playerY, graveZ - 2);
        controller.inTask = false;
    }
}
