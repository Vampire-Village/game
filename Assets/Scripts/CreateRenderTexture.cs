using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture;

    private Camera minimapCamera;

    // delete these later
    public GameObject UIcanvas;
    public GameObject render;

    // Start is called before the first frame update
    void Start()
    {
        minimapCamera = GetComponent<Camera>();

        renderTexture = new RenderTexture(150, 150, 0, RenderTextureFormat.ARGB32);
        renderTexture.name = "please work";
        
        renderTexture.Create();

        minimapCamera.targetTexture = renderTexture;
    }

    public void AssignRenderTexture(int type)
    {
        UIcanvas = GameObject.Find("UI Canvas").gameObject;
        if (type == 0) // Vampire or Infected
        {
            render = UIcanvas.transform.Find("VampireMinimap/Minimap/MinimapRender").gameObject;
            render.GetComponent<RawImage>().texture = renderTexture;
        }
        else if (type == 1) // Villager
        {
            GameObject render = UIcanvas.transform.Find("VillagerMinimap/Minimap/MinimapRender").gameObject;
            render.GetComponent<RawImage>().texture = renderTexture;
        }
        else
        {
            Debug.Log("Invalid argument to AssignRenderTexture");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
