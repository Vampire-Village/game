using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DustController : Task
{
    public GameObject dust;

    public bool taskComplete = false;
    public Item completeItem;

    private float dustCompletion = 0;
    public float dustGoal = 300;
    private bool mouseHeld = false;
    private float mouseChange = 0;
    private Image dustImg;
 

    // Start is called before the first frame update
    void Start()
    {
        dustImg = dust.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mouseChange = Mathf.Abs(Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y"));
            mouseHeld = true;
        }
        else
        {
            mouseHeld = false;
        }
    }

    public void OnGUI()
    {
        if (mouseHeld && !taskComplete)
        {
            dustCompletion += mouseChange*0.5f;
            dustImg.color = new Color(dustImg.color.r, dustImg.color.g, dustImg.color.b, 1 - (dustCompletion/dustGoal));

            if (dustCompletion > dustGoal)
            {
                taskComplete = true;
                CompleteTask(gameObject, completeItem);
            }
        }
    }
}
