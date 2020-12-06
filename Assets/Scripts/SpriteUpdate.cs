using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUpdate : MonoBehaviour
{
    public bool hair = false;
    public Sprite frontSprite;
    public Sprite backSprite;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject player = transform.root.gameObject;
        if (player.GetComponent<Controller>().isFrontFacing == true)
        {
            GetComponent<SpriteRenderer>().sprite = frontSprite;
            if (hair)
            {
                GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        } else
        {
            GetComponent<SpriteRenderer>().sprite = backSprite;
            if (hair)
            {
                GetComponent<SpriteRenderer>().sortingOrder = 5;
            }
        }
    }
}
