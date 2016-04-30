using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EntityMotor))]
public class PlayerController : MonoBehaviour {

    public float RunSpeed = 4f;
    public Vector3 JumpForce = new Vector3(0f, 400f, 0f);
    public Transform Camera;

    private EntityMotor m_motor;
    private CameraAim m_aim;

    private Vector3 m_forward;
    private Vector3 m_right;
    
    void Awake() {
        m_motor = GetComponent<EntityMotor>();
        m_aim = GetComponent<CameraAim>();
    }
	
	// Update is called once per frame
	void Update () {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        bool jump = Input.GetButtonDown("Jump");

        m_forward = Vector3.ProjectOnPlane(Camera.transform.forward, transform.up);
        m_right = Vector3.ProjectOnPlane(Camera.transform.right, transform.up);

        Vector3 move = Vector3.ProjectOnPlane(RunSpeed * (m_forward * vertical + m_right * horizontal), m_motor.GroundNormal);

        m_motor.Move(move, jump ? JumpForce : Vector3.zero);
    }
}
