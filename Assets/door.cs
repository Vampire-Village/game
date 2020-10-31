using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : Interactable
{
    // Start is called before the first frame update
    private bool closing = true;

    void FixedUpdate()
    {
        if (!closing)
        {
            if(transform.rotation.y > 90)
            {
                transform.Rotate(0.0f, 3.0f, 0.0f, Space.Self);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
        }
        else
        {
            if (transform.rotation.y > 0)
            {
                transform.Rotate(0.0f, -3.0f, 0.0f, Space.Self);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
        }
    }


    public override void Interact()
    {
        closing = !closing;
    }
}
