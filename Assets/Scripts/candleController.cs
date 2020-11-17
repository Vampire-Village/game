using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class candleController : Task
{
    public GameObject PourButton;
    public GameObject candle;
    public float pourRate;
    private float candleTop;
    public bool taskComplete = false;
    public string completeItem = "CandleBundle";
    // Start is called before the first frame update
    public RectTransform heightReference;
    public void Start()
    {
        candleTop = -375.0f;
        heightReference = candle.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    public void Update()
    {
        bool buttonStatus = PourButton.GetComponent<mouseHoldController>().isMouseHeld();
        if (buttonStatus && !taskComplete)
        {
            candleTop += (pourRate * Time.deltaTime);
            heightReference.offsetMax = new Vector2(heightReference.offsetMax.x, candleTop);
            if(candleTop > -225.0f)
            {
                taskComplete = true;
                completeTask(gameObject, completeItem);
            }
            //Debug.Log(candleTop);
        }
    }

 


}
