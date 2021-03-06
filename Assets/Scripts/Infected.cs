using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infected : MonoBehaviour
{

    public GameObject pingSprite;

    private float xPos;
    private float zPos;

    private GameObject minimap;

    private int cooldown = 5;
    private int cdSec = 0;

    private PlayerUI playerUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void RegisterUI(PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }

    public void Report()
    {
        //if (cdSec == 0)
        //{
            //cdSec = cooldown;
            //StartCoroutine(LowerCooldown());
            Debug.Log("*ping*");
            minimap = GameObject.Find("/UI Canvas/VampireMinimap"); // move to start when testing actual game
            xPos = GetComponent<Transform>().position.x;
            zPos = GetComponent<Transform>().position.z;
            // may need to adjust y/z positions
            GameObject ping = Instantiate(pingSprite, new Vector3(xPos, 11, zPos + 1.1f), Quaternion.Euler(90, 0, 0)) as GameObject;
            //Instantiate(pingSprite, new Vector3(xPos, 11, zPos + 1.1f), Quaternion.Euler(90, 0, 0));
            ping.transform.parent = minimap.transform;
        //}
    }

    IEnumerator LowerCooldown()
    {
        while (cdSec > 0)
        {
            // Change button text
            playerUI.reportText.text = "Report (" + cdSec + ")";

            yield return new WaitForSeconds(1);
            cdSec -= 1;
        }
        playerUI.reportText.text = "Report";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
