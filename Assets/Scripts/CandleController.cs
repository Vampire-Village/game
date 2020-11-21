using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CandleController : Task
{
    public GameObject PourButton;
    public GameObject candle;
    public float pourRate;
    private float candleTop;
    public bool taskComplete = false;
    public Item completeItem;
    public RectTransform heightReference;

    public GameObject playerReference;

    public void Start()
    {
        candleTop = 0.0f;
        heightReference = candle.GetComponent<RectTransform>();
        //playerReference = TaskSpawner.currentPlayer();
    }

    public void OnGUI()
    {
        bool buttonStatus = PourButton.GetComponent<MouseHoldController>().isMouseHeld();
        if (buttonStatus && !taskComplete)
        {
            candleTop += (pourRate * Time.deltaTime);
            heightReference.sizeDelta = new Vector2(100, candleTop);
            if(candleTop > 150.0f)
            {
                taskComplete = true;
                completeTask(gameObject, completeItem);
            }
            //Debug.Log(candleTop);
        }
    }

 


}
