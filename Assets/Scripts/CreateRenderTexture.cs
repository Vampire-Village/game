using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Experimental.Rendering;

public class CreateRenderTexture : MonoBehaviour
{
    public RenderTexture renderTexture;

    private Camera minimapCamera;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AssignRenderTexture(int type)
    {
        GameObject UIcanvas = GameObject.Find("UI Canvas").gameObject;
        if (type == 0) // Vampire or Infected
        {
            minimapCamera = GetComponent<Camera>();
            renderTexture = new RenderTexture(150, 150, 0, RenderTextureFormat.ARGB32);
            minimapCamera.targetTexture = renderTexture;

            GameObject render = UIcanvas.transform.Find("VampireMinimap/Minimap/MinimapRender").gameObject;
            render.GetComponent<RawImage>().texture = renderTexture;
        }
        else if (type == 1) // Villager
        {
            
            minimapCamera = GetComponent<Camera>();
            renderTexture = new RenderTexture(150, 150, 0, RenderTextureFormat.ARGB32);
            minimapCamera.targetTexture = renderTexture;

            GameObject render = UIcanvas.transform.Find("VillagerMinimap/Minimap/MinimapRender").gameObject;
            render.GetComponent<RawImage>().texture = renderTexture;
        }
        else
        {
            Debug.Log("Invalid argument to AssignRenderTexture");
        }
    }
}
