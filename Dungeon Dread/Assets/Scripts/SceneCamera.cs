using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamera : MonoBehaviour
{

    public float movementSpeed;

    public float lookSensitivity;

    float xRot;
    float yRot;

    private void Start()
    {
        xRot = 0;
        yRot = 0;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)) movementSpeed = 5f;
        else movementSpeed = 10f;

        xRot += Input.GetAxisRaw("Mouse X") * lookSensitivity;
        yRot -= Input.GetAxisRaw("Mouse Y") * lookSensitivity;

        transform.eulerAngles = new Vector3(yRot, xRot, 0);


        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        Vector3 localRight= transform.worldToLocalMatrix.MultiplyVector(transform.right);

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(localForward * movementSpeed * Time.deltaTime,Space.Self);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(-localForward * movementSpeed * Time.deltaTime);

        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(localRight * movementSpeed * Time.deltaTime);

        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-localRight * movementSpeed * Time.deltaTime);

        }
    }

    
}
