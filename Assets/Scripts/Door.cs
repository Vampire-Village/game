using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public GameObject door;
    public GameObject hinge;
    private bool closing = true;
    private Vector3 hingePos;
    public float rotationSpeed = 60.0f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool moving = false;
    private Quaternion openBase;
    private LockerSystem lockerSystem;
    private AudioSource audioSource;
    
    void Start()
    {
        lockerSystem = GetComponent<LockerSystem>();
        audioSource = GetComponent<AudioSource>();

        originalPosition = door.transform.position;
        originalRotation = door.transform.rotation;
        hingePos = hinge.transform.position;
        openBase = Quaternion.Euler(originalRotation.eulerAngles.x, 90.0f + originalRotation.eulerAngles.y, originalRotation.eulerAngles.z);
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
            while (Quaternion.Angle(door.transform.rotation, originalRotation) < 90.0f)
            {
                moving = true;
                door.transform.RotateAround(hingePos, Vector3.up, rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while (Quaternion.Angle(openBase, door.transform.rotation) < 90.0f)
            {
                moving = true;
                door.transform.RotateAround(hingePos, Vector3.up, -rotationSpeed *Time.deltaTime);
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
        audioSource.Play();
        yield return SetDoorAsync(true);
        yield return SetDoorAsync(false);
    }
}
