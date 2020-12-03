using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireLord : MonoBehaviour
{

    private List<GameObject> villagersInRange = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void Infect()
    {
        if (!(villagersInRange.Count == 0))
        {
            Debug.Log("BITE!!");
            GameObject victim = GetClosestVillager(villagersInRange);
            victim.GetComponent<Villager>().Infected();
            villagersInRange.Remove(victim);
        } else
        {
           
            Debug.Log("Nobody to infect.");
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






    // Update is called once per frame
    void Update()
    {
        
    }
}
