using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
//ATTACH SCRIPT TO THE VAMPIRE LORD PREFAB. This script will activate/deactivate trail renderers of player of tag "Player" depending on their distance to the vampire lord.
//NOTE: CURRENTLY DOES NOT SUPPORT PLAYERS THAT JOIN THE SCENE AFTER THIS SCRIPT HAS STARTED. Will change if needed.
public class BloodHunt : MonoBehaviour
{
    public int range = 10;
    public GameObject[] playersInScene;
    public GameObject[] playerInRange;
    public TrailRenderer trail;
    // Start is called before the first frame update
    void Start()
    {
        playersInScene = GameObject.FindGameObjectsWithTag("Player");
        Initialize_Trails();
    }

    // Update is called once per frame
    void Update()
    {
        Update_Player_In_Range();
        RenderBlood();
    }
    void Update_Player_In_Range()
    {
        //Updates the player within range, from the list of all players loaded at the beginning of scene.
        playerInRange = (from player in playersInScene where (Vector3.Distance(player.transform.position, this.transform.position) < range) select player).ToArray();
    }
    void Initialize_Trails()
    {
        //UNFINISHED: Initialization of trails for all players. Considering adding trail to villager prefab instead. 
        foreach(GameObject player in playersInScene)
        {
            TrailRenderer tr = player.AddComponent<TrailRenderer>();
            //Copy the trail renderer onto all players

        }
    }
    void RenderBlood()
    {   
        //Changes Player Trail Render to either True or False depending if within range.
        foreach (GameObject player in playersInScene)
        {
            if (playerInRange.Contains(player)){
                player.GetComponent<TrailRenderer>().enabled = true;
            }
            else
            {
                player.GetComponent<TrailRenderer>().enabled = false;
            }
        }
    }
}
