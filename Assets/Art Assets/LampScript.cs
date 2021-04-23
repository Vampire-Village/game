using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unfinished, have no clue how to get this material swapping to work.
/// </summary>
public class LampScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Material onMaterial;
    public Material offMaterial;
    public MeshRenderer lamp;
    public GameObject light;
    bool lighton = true;
    void Start()
    {
        lamp = GetComponent<MeshRenderer>();
        lamp.materials[1] = onMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        //for testing
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Switch();
        }
    }
    void Switch()
    {
        lighton = !lighton;
        if (lighton)
        {
            lamp.materials[1] = offMaterial;
            light.SetActive(false);
        }
        else
        {
            lamp.materials[1] = onMaterial;
            light.SetActive(true);
        }
    }

}
