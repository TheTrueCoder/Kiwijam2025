using System.Collections.Generic;
using UnityEngine;


public class movement : MonoBehaviour
{
    public GameObject playerCamera;
    public Rigidbody rb;

    public float _speed = 10f;
    private Vector2 _input;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    private Vector3 _movementVector;

    float rotationX = 0;


    public bool canMove = true;
    public bool canLook = true;


    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = !UnityEngine.Cursor.visible;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {


            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            Application.Quit();
        }
        //Get input direction
        _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        _input = _input.normalized;
        if (canMove == true)
        {

            //Movement
            _movementVector = _input.x * transform.right * _speed + _input.y * transform.forward * _speed;
            rb.linearVelocity = new Vector3(_movementVector.x, rb.linearVelocity.y, _movementVector.z);
        }
        if (canLook == true)
        {            //Rotation
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);


        }

    }


    public void BeStill()
    {
        canLook = false;
        canMove = false;
    }
}