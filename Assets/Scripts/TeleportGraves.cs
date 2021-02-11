using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGraves : Interactable
{
    public GameObject teleportTo;
    public int secondsToTeleport = 1;

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
    }

    IEnumerator WaitToTeleport(GameObject player)
    {
        Controller controller = player.GetComponent<Controller>();
        controller.inTask = true;
        yield return new WaitForSeconds(secondsToTeleport);
        float playerY = controller.transform.position.y;
        controller.transform.position = new Vector3(graveX, playerY, graveZ - 2);
        controller.inTask = false;
    }
}
