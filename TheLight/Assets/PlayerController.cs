﻿using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;


    private PlayerMotor motor;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }
    
    void Update()
    {
        //Calculate movement velocity as 3d vector
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov;
        Vector3 movVertical = transform.forward * zMov;

        //Final movement vector
        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;

        //Apply movement
        motor.Move(velocity);

        //Calculate rotation as 3d vector for turning only
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

        //Apply rotation
        motor.Rotate(rotation);

        //Calculate camera pitch as 3d vector for looking up and down
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 cameraPitch = new Vector3(xRot, 0f, 0f) * lookSensitivity;

        //Apply camera pitch
        motor.PitchCamera(cameraPitch);

    }
}
