using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
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

    public override void Interact(GameObject player)
    {
        if (!moving)
        {
            moving = true;
            closing = !closing;
            SetDoor(!closing);
        }
    }

    public void SetDoor(bool shouldOpen)
    {
        StartCoroutine(SetDoorAsync(!closing));
    }

    private IEnumerator SetDoorAsync(bool shouldOpen)
    {
        if (shouldOpen)
        {
            while (Quaternion.Angle(transform.rotation, originalRotation) < 90.0f)
            {
                moving = true;
                transform.RotateAround(hingePos, Vector3.up, rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while ((Quaternion.Angle(openBase, transform.rotation) < 90.0f))
            {
                moving = true;
                transform.RotateAround(hingePos, Vector3.up, -rotationSpeed *Time.deltaTime);
                yield return null;
            }
        }
        moving = false;
    }
}
