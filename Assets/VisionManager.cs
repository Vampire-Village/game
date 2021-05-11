using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using VampireVillage.Network;
public class VisionManager : MonoBehaviour
{
    //Attach this script to the parent prefab. Make sure to assign the parentSprite via drag and click!
    // Start is called before the first frame update
    public Camera camera;
    public GamePlayer gamePlayer;
    public Transform parentSprite;
    int ghostViewable = 11;
    int vampireViewable = 10;
    void Start()
    {
        camera = this.GetComponent<Camera>();
        //Unsure if line above works
        GamePlayer.OnPlayerSpawned.AddListener(InitializePlayer);
    }
    void InitializePlayer()
    {
        gamePlayer = GamePlayer.local;
        gamePlayer.OnRoleUpdated.AddListener(UpdateVisualsForRole);
        parentSprite = GamePlayer.local.transform;
    }
    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateVisualsForRole();
        }
        */
    }

    void SetSpriteLayer(LayerMask layer)
    {
        //set children of the parent sprite to specific layer
        parentSprite.gameObject.layer = layer;
        foreach(Transform sprite in parentSprite)
        {
            sprite.gameObject.layer = layer;
        }
    }
    void UpdateVisualsForRole()
    {
        //Notes: Vampire Lords can see what villagers see + blood trails
        //Infected can see what villagers see

        switch (gamePlayer.role)
        {
            case Role.VampireLord:
                camera.cullingMask = ~(1 << ghostViewable);
                break;
            case Role.Ghost:
                SetSpriteLayer(ghostViewable);
                break;
            default:
                camera.cullingMask = ~((1 << ghostViewable) | (1 << vampireViewable));
                break;
        }
    }
}
