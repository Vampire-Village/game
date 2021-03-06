﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    private Light m_light;
    [SerializeField] [Range(0f,0.1f)] float transitionTime = 0;
    public Color activeColor;
    public Color dayLight;
    public Color nightLight;
    public Color activeFog;
    public Color dayFog;
    public Color nightFog;
    // Start is called before the first frame update
    void Start()
    {
        m_light = GetComponent<Light>();
        DayNight(false);
    }

    // Update is called once per frame
    void Update()
    {
        m_light.color = Color.Lerp(m_light.color, activeColor, transitionTime);
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, activeFog, transitionTime);
    }

    public void DayNight(bool isDay)
    {
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
