using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
//ATTACH SCRIPT TO THE VAMPIRE LORD PREFAB. This script will activate/deactivate trail renderers of player of tag "Player" depending on their distance to the vampire lord.
//NOTE: CURRENTLY DOES NOT SUPPORT PLAYERS THAT JOIN THE SCENE AFTER THIS SCRIPT HAS STARTED. Will change if needed. Fixed 4/19, might be inefficient fix though.
//Script must be activated during role assignment
public class BloodHunt : MonoBehaviour
{
    public int range = 10;
    public int secondsBloodLasts = 5;
    public GameObject[] playersInScene;
    public GameObject[] playerInRange;
    public TrailRenderer trail;
    // Start is called before the first frame update
    void Start()
    {
        playersInScene = GameObject.FindGameObjectsWithTag("Player");
        Initialize_Blood_settings();
    }

    // Update is called once per frame
    void Update()
    {
        playersInScene = GameObject.FindGameObjectsWithTag("Player"); //Constantly updates this Array, may not be efficient
        Update_Player_In_Range();
        RenderBlood();
    }
    void Initialize_Blood_settings()
    {
        foreach (GameObject player in playersInScene)
        {
            TrailRenderer trail = player.GetComponentInChildren<TrailRenderer>();
            trail.time = secondsBloodLasts;
        }
    }
    void Update_Player_In_Range()
    {
        //Updates the player within range, from the list of all players loaded at the beginning of scene.
        playerInRange = (from player in playersInScene where (Vector3.Distance(player.transform.position, this.transform.position) < range && player.GetComponent<GamePlayer>().villager.enabled == true) select player).ToArray();
        //Unsure if the above functions correctly to only add players in if they are villagers
    }
    
    void RenderBlood()
    {   
        //Changes Player Trail Render to either True or False depending if within range.
        foreach (GameObject player in playersInScene)
        {
            TrailRenderer trail = player.GetComponentInChildren<TrailRenderer>();
            trail.enabled = true;
            if (playerInRange.Contains(player)){
                trail.emitting = true;
            }
            else
            {
                trail.emitting = false;
            }
        }
    }
}
