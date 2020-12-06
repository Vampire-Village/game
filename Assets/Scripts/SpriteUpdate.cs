using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteUpdate : MonoBehaviour
{
    public bool hair = false;
    public Sprite frontSprite;
    public Sprite backSprite;

    private Controller controller;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        controller = transform.root.gameObject.GetComponent<Controller>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isFrontFacing == true)
        {
            spriteRenderer.sprite = frontSprite;
            if (hair)
            {
                spriteRenderer.sortingOrder = 0;
            }
        } else
        {
            spriteRenderer.sprite = backSprite;
            if (hair)
            {
                spriteRenderer.sortingOrder = 5;
            }
        }
    }
}
