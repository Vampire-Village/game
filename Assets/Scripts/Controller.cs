using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    //public Button interactButton;
    private bool inInteractableRange = false;
    private GameObject interactable;
    public bool inTask = false;
    void Start()
    {
        //Button btn = interactButton.GetComponent<Button>();
    }
    void Update()
    {
        if (inInteractableRange && (Input.GetButtonDown("Interact")))
        {
            interactable.GetComponent<Interactable>().Interact();
        }
    }
    void FixedUpdate()
    {
        float moveXAxis = Input.GetAxis("Horizontal");
        float moveZAxis = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveXAxis, 0, moveZAxis);
        movement = Vector3.ClampMagnitude(movement, 1f);
        if (!inTask)
        {
            transform.Translate(movement * speed * Time.deltaTime);
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
    void OnTriggerExit()
    {
        inInteractableRange = false;
    }

    public void startTask()
    {
        inTask = true;
    }
    public void endTask()
    {
        inTask = false;
    }
    
}
