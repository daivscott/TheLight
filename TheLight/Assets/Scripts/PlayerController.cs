using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    [SerializeField]
    private float jumpForce = 1000f;

    //[SerializeField]
    //private float healthBarRegenSpeed = 1f;

    [Header("Spring Settings")]
    //[SerializeField]
    //private JointDriveMode jointMode;
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSettings(jointSpring);
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

        float cameraPitchX = xRot * lookSensitivity;

        //Apply camera pitch
        motor.PitchCamera(cameraPitchX);


        /// calculate jump force based on player input
        Vector3 _jumpforce = Vector3.zero;        
        if(Input.GetButton("Jump"))
        {
            _jumpforce = Vector3.up * jumpForce;
            SetJointSettings(0f);
        } else
        {
            SetJointSettings(jointSpring);
        }

        //apply jump force
        motor.ApplyJump(_jumpforce);

    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = _jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
