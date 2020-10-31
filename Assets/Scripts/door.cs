using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : Interactable
{
    // Start is called before the first frame update
    private bool closing = true;
    private Vector3 hingePos;
    private GameObject hinge;
    public float rotationSpeed = 60.0f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool moving = false;
    private Quaternion openBase;
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        hinge = transform.GetChild(0).gameObject;
        hingePos = hinge.transform.position;
        openBase = Quaternion.Euler(0.0f, 90.0f + originalRotation.eulerAngles.y, 0.0f);
    }
    void Update()
    {
        if (!closing)
        {
            if(Quaternion.Angle(transform.rotation, originalRotation) < 90.0f)
            {
                transform.RotateAround(hingePos, Vector3.up, rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0.0f, 90.0f + originalRotation.eulerAngles.y, 0.0f);
                moving = false;
            }
        }
        else
        {
            if (Quaternion.Angle(openBase, transform.rotation) < 90.0f)
            {
                transform.RotateAround(hingePos, Vector3.up, -rotationSpeed*Time.deltaTime);
            }
            else
            {
                //transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                transform.SetPositionAndRotation(originalPosition, originalRotation);
                moving = false;
            }
        }
        //Debug.Log(originalRotation);
        //Debug.Log(Quaternion.Angle(transform.rotation, originalRotation));
    }


    public override void Interact()
    {
        if (!moving)
        {
            moving = true;
            closing = !closing;
        }
    }
}
