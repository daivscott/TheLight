using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraPitchX = 0f;
    private float currentCameraPitchX = 0f;
    private Vector3 jumpForce = Vector3.zero;

    [SerializeField]
    private float cameraLimitRotation = 85f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    //get movement vector
    public void Move (Vector3 _velocity)
    {
        velocity = _velocity;
    }

    //get rotate vector
    public void Rotate(Vector3 _rotation)
    {
        rotation  = _rotation;
    }

    //get pitch vector
    public void PitchCamera(float _cameraPitchX)
    {
        cameraPitchX = _cameraPitchX;
    }

    // Get a force vector for our jump
    public void ApplyJump(Vector3 _jumpforce)
    {
        jumpForce = _jumpforce;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(jumpForce != Vector3.zero)
        {
            rb.AddForce(jumpForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            // Set rotation and clamp it 
            currentCameraPitchX -= cameraPitchX;
            currentCameraPitchX = Mathf.Clamp(currentCameraPitchX, -cameraLimitRotation, cameraLimitRotation);

            // Apply rotation to the transform of camera
            cam.transform.localEulerAngles = new Vector3(currentCameraPitchX, 0f, 0f);
        }
    }
    	
}
