using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraPitch = Vector3.zero;


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
    public void PitchCamera(Vector3 _cameraPitch)
    {
        cameraPitch = _cameraPitch;
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
    }

    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            cam.transform.Rotate(-cameraPitch);
        }
    }
    	
}
