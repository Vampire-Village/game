using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpawner : MonoBehaviour
{
    public bool isBleeding = true;
    public Transform player;
    public GameObject bloodPrefab; //need to manually drag the prefab in
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Transform>();
    }
    //STILL NEEDS: to randomize spawntimes, as well as randomize size
    // Update is called once per frame
    void Update()
    {
        if (isBleeding)
        {
            
            time += Time.deltaTime;
            if (time > 0.5)
            {
                SpawnBlood();
                time = 0;
            }
        }
        
    }
    RaycastHit ShootDownwardRaycast()
    {
        RaycastHit hit;
        Physics.Raycast(player.position, -Vector3.up, out hit);
        //Debug.DrawRay(player.position, -Vector3.up, new Color(1f, 0f, 0f, 1f));
        return hit;
    }
    void SpawnBlood()
    {
        Vector3 raycast = ShootDownwardRaycast().point;
        Instantiate(bloodPrefab, new Vector3(raycast.x, raycast.y + 0.05f, raycast.z), Quaternion.identity);
        
    }

}
