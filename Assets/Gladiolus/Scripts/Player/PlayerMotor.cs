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

    private CharacterController _characterController;
    private PlatformDetector _platformDetector;

    private Vector3 _forward;
    private Vector3 _right;

    public bool _isJumping = false;
    public bool _isGrounded = false;

    public float _platformSlope = 0f;
    public Vector3 _platformOffset;
    public Vector3 _platformNormal;
    public Vector3 _downhill;

    public Vector3 _runVelocity;
    public Vector3 _jumpVelocity;
    public Vector3 _slideVelocity;
    public Vector3 _gravityVelocity;
    public Vector3 _velocity;

    public bool DebugIsGrounded;

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
        _runVelocity = RunSpeed * (horizontal * _right + vertical * _forward);

        if (_characterController.isGrounded) {
            // Cancel falling velocity on landing
            _gravityVelocity = Vector3.zero;

            // Check if platform is steep enough to slide down
            if (_platformSlope >= SlideAngle) {
                _slideVelocity += _downhill * Gravity; //Vector3.Dot(-Vector3.up * Gravity, _downhill);
            } else {
                // Cancel sliding on shallow inclines
                _slideVelocity = Vector3.zero;
            }

            // Jump
            if (jump && !_isJumping) {
                _jumpVelocity = transform.up * JumpSpeed;
                _isJumping = true;
            } else {
                // Attach Player to platform
                platformFound = _platformDetector.CheckForPlatforms(out _platformOffset);

                if (platformFound) {
                    _runVelocity += _platformOffset;
                }

                _jumpVelocity = Vector3.zero;
                _isJumping = false;
            }
        } else {
            _slideVelocity = Vector3.zero;
            _gravityVelocity += Gravity * -Vector3.up;
        }

        _velocity = Time.deltaTime * (_runVelocity + _jumpVelocity + _gravityVelocity + _slideVelocity);
        _characterController.Move(_velocity);

        DebugIsGrounded = _characterController.isGrounded;
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
