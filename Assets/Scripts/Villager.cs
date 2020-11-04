using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void Infected() // Switches Villager to Infected
    {
        gameObject.AddComponent<Infected>();
        gameObject.tag = "Infected";
        Destroy(this);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
