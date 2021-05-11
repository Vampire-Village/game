using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//REFERENCE THIS SCRIPT AND USE PLAYANIMATION TO PLAY THE TELEPORT ANIMATION. THE CONTENTS OF THIS PARTICLE SYSTEM CAN BE ACTIVATED/DEACTIVATED
//DEPENDING ON WHICH ANIMATIONS YOUD LIKE TO SHOW/HAVE. COULD BE CUSTOMIZABLE IN THE FUTURE
//NOTE: CURRENTLY IS ACTIVATABLE WITH INPUT KEY SPACE SO DEACTIVATE THIS ONCE ADDED TO THE GRAVE SCRIPT.
public class TeleportAnimation : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimations();
        }
        */
    }
    public void PlayAnimations()
    {
        GetComponent<ParticleSystem>().Play(true);
    }
}
