using UnityEngine;

public class Zoom : MonoBehaviour
{
    public float sensitivity = 1;
    Camera m_camera;
    [HideInInspector]
    public float defaultFOV;
    [Tooltip("Effectively the min FOV that we can reach while zooming with this camera.")]
    public float maxZoom = 15;
    [HideInInspector]
    public float zoomAmount;


    void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    void Start()
    {
        defaultFOV = m_camera.fieldOfView;
    }

    void Update()
    {
        zoomAmount += Input.mouseScrollDelta.y * sensitivity * .05f;
        zoomAmount = Mathf.Clamp01(zoomAmount);
        m_camera.fieldOfView = Mathf.Lerp(defaultFOV, maxZoom, zoomAmount);
    }
}
