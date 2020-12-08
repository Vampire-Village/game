using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VampireLord : MonoBehaviour
{
    private List<GameObject> villagersInRange = new List<GameObject>();
    private int cooldown = 30;
    private int cdSec = 0;

    private PlayerUI playerUI;

    public void RegisterUI(PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }

    public void Infect()
    {
        if (cdSec == 0)
        {
            if (!(villagersInRange.Count == 0))
            {
                cdSec = cooldown;

                StartCoroutine(LowerCooldown());
                Debug.Log(cdSec);

                Debug.Log("BITE!!");
                GameObject victim = GetClosestVillager(villagersInRange);
                victim.GetComponent<Villager>().Infected();
                villagersInRange.Remove(victim);
            }
            else
            {
                Debug.Log("Nobody to infect.");
            }
        } 
    }

    GameObject GetClosestVillager(List<GameObject> villagers)
    {
        GameObject closestVillager = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject v in villagers)
        {
            float dist = Vector3.Distance(v.transform.position, currentPos);
            if (dist < minDist)
            {
                closestVillager = v;
                minDist = dist;
            }
        }
        return closestVillager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<GamePlayer>().role == Role.Villager)
            {
                villagersInRange.Add(other.gameObject);
                Debug.Log("Villager in range");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<GamePlayer>().role == Role.Villager)
            {
                villagersInRange.Remove(other.gameObject);
                Debug.Log("Villager lost");
            }
        }
    }

    IEnumerator LowerCooldown()
    {
        while (cdSec > 0)
        {
            // Change button text
            playerUI.infectText.text = "Infect (" + cdSec + ")";

            yield return new WaitForSeconds(1);
            cdSec -= 1;
        }
        playerUI.infectText.text = "Infect";
    }
}
