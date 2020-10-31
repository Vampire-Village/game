using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        float moveXAxis = Input.GetAxis("Horizontal");
        float moveZAxis = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveXAxis, 0, moveZAxis);
        movement = Vector3.ClampMagnitude(movement, 1f);
        transform.Translate(movement * speed * Time.deltaTime);
    }
}
