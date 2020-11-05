using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mouseHoldController : Selectable
{
    // Start is called before the first frame update
 

    // Update is called once per frame
    public bool isMouseHeld()
    {
        return IsPressed();
    }


}
