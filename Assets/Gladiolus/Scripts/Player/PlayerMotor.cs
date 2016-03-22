using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlatformDetector))]
public class PlayerMotor : MonoBehaviour {
    public Transform Pivot;

    public float RunSpeed = 10f;
    public float WalkSpeed = 10f;
    public float JumpSpeed = 100f;
    public float Gravity = 9.81f;
    public int SlideAngle = 60;

    public LayerMask PlatformLayers;

    public bool DebugIsGrounded;
    public bool DebugPlatformFound;
    public Vector3 DebugJumpVector;
    public Vector3 DebugFallCancel;

    private CharacterController _characterController;
    private PlatformDetector _platformDetector;

    private Vector3 _forward;
    private Vector3 _right;

    public Vector3 _offset;
    public Vector3 _velocity;
    public Vector3 _acceleration;

    public bool _isJumping = false;

    public float _platformSlope = 0f;
    public Vector3 _platformOffset;
    public Vector3 _platformNormal;
    public Vector3 _downhill;

	void Awake() {
        _characterController = GetComponent<CharacterController>();
        _platformDetector = GetComponent<PlatformDetector>();
	}
    
    void Update() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");

        Move(horizontal, vertical, jump);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if ((PlatformLayers.value & (1 << hit.gameObject.layer)) != 0) {
            // TODO Check if hit on bottom of collider
            _platformNormal = hit.normal;
            _platformSlope = Vector3.Angle(_platformNormal, transform.up);
            _downhill = Vector3.Cross(_platformNormal, Vector3.Cross(_platformNormal, transform.up)).normalized;
        }
    }

    public void Move(float horizontal, float vertical, bool jump) {
        bool platformFound = false;

        _forward = new Vector3(Pivot.transform.forward.x, 0f, Pivot.transform.forward.z).normalized;
        _right = new Vector3(Pivot.right.x, 0f, Pivot.right.z).normalized;
        _offset = RunSpeed * Time.deltaTime * (horizontal * _right + vertical * _forward);

        _acceleration = Vector3.zero;

        if (_characterController.isGrounded) {
            // Linecast downward to check for platforms
            platformFound = _platformDetector.CheckForPlatforms(out _platformOffset);

            // Check if platform is shallow enough to stop falling
            if (_platformSlope < SlideAngle) {
                // Cancel falling velocity on landing
                Vector3 fallCancel = Vector3.Dot(_velocity, -Vector3.up) * Vector3.up;
                DebugFallCancel = fallCancel;
                _velocity += fallCancel;
            } else {
                _acceleration += -transform.up * Gravity;
            }

            // Jump
            if (jump && !_isJumping) {
                _acceleration = transform.up * JumpSpeed;
                DebugJumpVector = _acceleration * Time.deltaTime;
                _isJumping = true;
            } else {
                // Attach Player to platform
                if (platformFound) {
                    _offset += _platformOffset;
                }
                _isJumping = false;
            }
        } else {
            DebugFallCancel = Vector3.zero;
            _acceleration += -transform.up * Gravity;
        }

        _velocity += _acceleration * Time.deltaTime;
        _characterController.Move(_velocity + _offset);

        DebugIsGrounded = _characterController.isGrounded;
        DebugPlatformFound = platformFound;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _forward);
        Gizmos.DrawLine(transform.position, transform.position + _right);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + _platformNormal);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + _downhill);
    }
}
