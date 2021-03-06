using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infected : MonoBehaviour
{

    public GameObject pingSprite;

    private float xPos;
    private float zPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Report()
    {
        xPos = GetComponent<Transform>().position.x;
        zPos = GetComponent<Transform>().position.z;
        // make sure this has a cooldown
        // spawn ping on minimap
        // erase ping after a few seconds
        Debug.Log("*ping*");
        // may need to adjust y/z positions
        Instantiate(pingSprite, new Vector3(xPos, 11, zPos + 1.1f), Quaternion.Euler(90, 0, 0));
    } 


   
    // Update is called once per frame
    void Update()
    {
        
    }
}
