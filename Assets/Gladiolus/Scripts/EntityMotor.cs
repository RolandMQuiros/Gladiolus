using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class EntityMotor : MonoBehaviour {
    public float GROUND_CAST_DISTANCE = 0.1f;

    public Vector3 Gravity = new Vector3(0f, -9.81f, 0f);
    public float JumpForce = 400f;
    public LayerMask GroundMask;

    public bool IsGrounded;
    public Vector3 GroundNormal;

    public float test_runspeed = 8f;

    private Rigidbody m_rigidbody;
    private CapsuleCollider m_collider;

    // Use this for initialization
    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();
    }

    void Update() {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Move(test_runspeed * move, Input.GetButtonDown("Jump"));
    }

    void FixedUpdate() {
        m_rigidbody.AddForce(Gravity);
        GroundCheck();
    }

    void GroundCheck() {
        RaycastHit hitInfo;
        Vector3 gravityNormal = Gravity.normalized;

        float maxDistance = Vector3.Dot(m_rigidbody.velocity, gravityNormal);
        Debug.DrawLine(
            transform.position - GROUND_CAST_DISTANCE * gravityNormal, 
            transform.position + maxDistance * gravityNormal, 
            Color.red
        );

        Ray groundCast = new Ray(transform.position - GROUND_CAST_DISTANCE * gravityNormal, gravityNormal);

        if (IsGrounded = Physics.Raycast(groundCast, out hitInfo, 1f, GroundMask.value)) {
            GroundNormal = hitInfo.normal;
            Debug.DrawLine(transform.position, transform.position + hitInfo.normal, Color.cyan);
        } else {
            GroundNormal = Gravity.normalized;
        }
    }

    public void Move(Vector3 velocity, bool jump) {
        Vector3 gravityDirection = Gravity.normalized;
        if (velocity != Vector3.zero) {
            // Remove old movement velocity
            Vector3 movementVelocity = Vector3.ProjectOnPlane(m_rigidbody.velocity, GroundNormal);
            Vector3 nonmovementVelocity = m_rigidbody.velocity - movementVelocity;
            Vector3 newMovementVelocity = Vector3.ProjectOnPlane(velocity, GroundNormal);

            m_rigidbody.velocity = nonmovementVelocity + newMovementVelocity;
        }

        if (jump && IsGrounded) {
            m_rigidbody.AddForce(-JumpForce * gravityDirection);
        }
    }
}
