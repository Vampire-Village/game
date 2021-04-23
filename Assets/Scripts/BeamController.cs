using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool target = false;
    private bool upCheck = false;
    private MeshRenderer meshRenderer;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void OnDisable()
    {
        target = false;
        meshRenderer.enabled = false;
    }
    // Update is called once per frame

    void OnMouseOver()
    {
        meshRenderer.enabled = true;
    }
    void OnMouseExit()
    {
        if(!target)
            meshRenderer.enabled = false;
    }
    
    void OnMouseDown()
    {
        if(!upCheck)
            target = !target;
        upCheck = true;
        meshRenderer.enabled = false;
    }
    void OnMouseUp()
    {
        upCheck = false;
    }
    
}
