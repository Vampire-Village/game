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
            Debug.Log("BITE!!"); // infect closest object with "villager" tag and change to "infected"
            villagersInRange[0].GetComponent<Villager>().Infected(); // only accounts for first villager in range
            villagersInRange.RemoveAt(0);
        } else
        {
            Debug.Log("Nobody to infect.");
        }

    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Villager")
        {
            villagersInRange.Add(other.gameObject);
            Debug.Log("Villager in range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Villager")
        {
            villagersInRange.Remove(other.gameObject);
            Debug.Log("Villager lost");
        }
    }






    // Update is called once per frame
    void Update()
    {
        
    }
}
