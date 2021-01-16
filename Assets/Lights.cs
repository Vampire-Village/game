using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    public Light light;
    [SerializeField] [Range(0f,0.1f)] float transitionTime;
    public Color activeColor;
    public Color dayLight;
    public Color nightLight;
    public Color activeFog;
    public Color dayFog;
    public Color nightFog;
    bool isDay = true;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            DayNight();
        }
        light.color = Color.Lerp(light.color, activeColor, transitionTime);
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, activeFog, transitionTime);

    }

    public void DayNight()
    {
        isDay = !isDay;
        if (isDay)
        {
            activeColor = dayLight;
            activeFog = dayFog;
        }
        else
        {
            activeFog = nightFog;
            activeColor = nightLight;
        }
    }
}
