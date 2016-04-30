using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class EntityMotor : MonoBehaviour {
    public float GROUND_CAST_START = -0.1f;
    public float GROUND_CAST_DISTANCE = 0.15f;

    public Vector3 Gravity = new Vector3(0f, -20f, 0f);

    public LayerMask GroundMask;
    public float GroundCheckSkin = 0.1f;

    public PhysicMaterial AirborneMaterial;
    public PhysicMaterial GroundedMaterial;

    public bool IsGrounded;
    public Vector3 GroundNormal;

    private Rigidbody m_rigidbody;
    private SphereCollider m_collider;

    //private Vector3 m_gravity = new Vector3(0f, -20f, 0f);
    private Vector3 m_gravityNormal;
    private Vector3 m_collisionNormal;
    private float m_groundAngle;
    private bool m_canJump = false;
    
    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        m_collider = GetComponent<SphereCollider>();

        m_gravityNormal = Gravity.normalized;
    }

    void FixedUpdate() {
        m_rigidbody.AddForce(Gravity, ForceMode.Acceleration);
        GroundCheck();
    }

    void GroundCheck() {
        RaycastHit hitInfo;
        Ray groundCast = new Ray(transform.position + m_collider.center, m_gravityNormal);

        if (Physics.SphereCast(groundCast, m_collider.radius - GroundCheckSkin, out hitInfo, 10f, GroundMask.value)) {
            GroundNormal = hitInfo.normal;
            IsGrounded = hitInfo.distance < GROUND_CAST_DISTANCE;
        } else {
            GroundNormal = -m_gravityNormal;
            IsGrounded = false;
        }

        if (IsGrounded) {
            m_collider.material = GroundedMaterial;
        } else {
            m_collider.material = AirborneMaterial;
        }
    }

    public void Move(Vector3 velocity, Vector3 jumpForce) {
        if (velocity != Vector3.zero) {
            Vector3 movementVelocity = Vector3.ProjectOnPlane(m_rigidbody.velocity, m_gravityNormal);
            Vector3 nonmovementVelocity = m_rigidbody.velocity - movementVelocity;
            Vector3 newMovementVelocity = Vector3.ProjectOnPlane(velocity, m_gravityNormal);

            m_rigidbody.velocity = newMovementVelocity + nonmovementVelocity;
        }

        if (IsGrounded && jumpForce != Vector3.zero) {
            IsGrounded = false;
            m_rigidbody.AddForce(jumpForce);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
    }
}
