using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;


    Vector3 dir;
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        float xMov = Input.GetAxisRaw("Horizontal") * movementSpeed;
        float yMov = Input.GetAxisRaw("Vertical") * movementSpeed;

        dir = xMov * transform.right + yMov * transform.forward;

    }

    private void FixedUpdate()
    {
        Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);

        rb.velocity = dir * Time.deltaTime;

        rb.velocity += yVelFix;
    }
}
