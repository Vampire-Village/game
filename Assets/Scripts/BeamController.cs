using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamController : MonoBehaviour
{
    // Start is called before the first frame update
    public bool target = false;
    private bool upCheck = false;
    private MeshRenderer meshRenderer;
    int layermask = 1 << 12;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        gameObject.SetActive(false);
        //layermask = ~layermask;
    }
    void OnDisable()
    {
        target = false;
        meshRenderer.enabled = false;
        upCheck = false;
    }
    Ray ray;
    RaycastHit hit;
    
    
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000, layermask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!upCheck)
                    meshRenderer.enabled = true;
                if (Input.GetMouseButtonDown(0))
                {
                    if (!upCheck)
                        target = !target;
                    upCheck = true;
                    meshRenderer.enabled = false;
                }
                else
                {
                    upCheck = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    target = false;
                }
                if (!target)
                    meshRenderer.enabled = false;
            }
        }
        else
        {
            if (!target)
                meshRenderer.enabled = false;
        }

    }
    // Update is called once per frame
    /*
    void OnMouseOver()
    {
        Debug.Log("wheeee");
        if(!upCheck)
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
    */
    
}
