using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    private bool closing = true;
    private Vector3 hingePos;
    private GameObject hinge;
    public float rotationSpeed = 60.0f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool moving = false;
    private Quaternion openBase;
    private LockerSystem lockerSystem;
    
    void Start()
    {
        lockerSystem = GetComponent<LockerSystem>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        hinge = transform.GetChild(0).gameObject;
        hingePos = hinge.transform.position;
        openBase = Quaternion.Euler(0.0f, 90.0f + originalRotation.eulerAngles.y, 0.0f);
    }

    public override void Interact(GameObject player)
    {
        if (!moving)
            lockerSystem.ActivateLocker();
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

    public void OpenCloseDoor()
    {
        StartCoroutine(OpenCloseDoorAsync());
    }

    private IEnumerator OpenCloseDoorAsync()
    {
        yield return SetDoorAsync(true);
        yield return SetDoorAsync(false);
    }
}
