using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Controller : NetworkBehaviour
{
    private Rigidbody rb;
    public float speed;
    private bool inInteractableRange = false;
    private GameObject interactable;
    public bool inTask = false;
    public Item heldItem;

    private PlayerAudio audioManager;

    [SyncVar]
    public bool isFrontFacing = true;

    void Start()
    {
        audioManager = GetComponent<PlayerAudio>();
    }

    void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetButtonDown("Interact"))
            {
                Interact();
            }
        }
    }
    void FixedUpdate()
    {
        if (hasAuthority)
        {
            float moveXAxis = Input.GetAxis("Horizontal");
            float moveZAxis = Input.GetAxis("Vertical");
            spriteFlip(moveZAxis);
            Vector3 movement = new Vector3(moveXAxis, 0, moveZAxis);
            movement = Vector3.ClampMagnitude(movement, 1f);

            if (!inTask)
            {
                transform.Translate(movement * speed * Time.deltaTime);
                if (moveXAxis != 0 || moveZAxis != 0)
                {
                    audioManager.PlayFootsteps();
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Interactable")
        {
            inInteractableRange = true;
            interactable = collider.gameObject;
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Interactable")
        {
            inInteractableRange = false;
        }
    }

    public void StartTask()
    {
        inTask = true;
    }
    public void EndTask(Item completeItem)
    {
        inTask = false;
        if (completeItem.displayName != "None")
        {
            heldItem = completeItem;
        }
        Debug.Log(heldItem.displayName);

    }
    public void Interact()
    {
        if (inInteractableRange)
        {
            interactable.GetComponent<Interactable>().Interact(gameObject);
        }
    }


    [Command]
    public void CmdSpriteUpdate(bool isFrontFacing)
    {
        this.isFrontFacing = isFrontFacing;
    }

    void spriteFlip(float moveZAxis)
    {
        if (moveZAxis > 0.01 && this.isFrontFacing != false)
        {
            CmdSpriteUpdate(false);
        }
        else if (moveZAxis < -0.01 && this.isFrontFacing != true)
        {
            CmdSpriteUpdate(true);
        }
    }

}
