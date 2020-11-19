using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseHoldController : Selectable
{
    // Start is called before the first frame update
 

    // Update is called once per frame
    public bool isMouseHeld()
    {
        return IsPressed();
    }


}
