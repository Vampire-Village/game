using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public SpriteRenderer bloodRender;
    public float fadeTime = 10f;
    private float currentAlpha = 255;
    private float alphaChange = 0;
    private float timeSinceChange = 0;
    // Start is called before the first frame update
    void Start()
    {
        bloodRender = GetComponentInChildren<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        alphaChange += (Time.deltaTime * 1 / fadeTime);
        currentAlpha = 1 - alphaChange;
        timeSinceChange += Time.deltaTime;
        if (timeSinceChange > 1)
        {
            bloodRender.color = new Color(bloodRender.color.r, bloodRender.color.g, bloodRender.color.b, currentAlpha);
            timeSinceChange = 0;
            if(currentAlpha <= 0)
            {
                Destroy(gameObject);
            }
                  
        }
        
    }
}
