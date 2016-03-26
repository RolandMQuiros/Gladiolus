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
    public int CeilingBumpAngle = 25;

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

    public Vector3 _ceilingNormal;
    public float _ceilingSlope = 0f;

    public Vector3 _runVelocity;
    public Vector3 _jumpVelocity;
    public Vector3 _slideVelocity;
    public Vector3 _gravityVelocity;

    public Vector3 _velocity;
    public Vector3 _offset;

    public bool DebugIsGrounded;
    public bool DebugCollidedSides;
    public bool DebugCollidedBottom;
    public bool DebugHit;

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
        bool isPlatform = (PlatformLayers.value & (1 << hit.gameObject.layer)) != 0;
        bool hitBelow = (_characterController.collisionFlags & CollisionFlags.Below) != 0;
        bool hitAbove = (_characterController.collisionFlags & CollisionFlags.Above) != 0;

        if (isPlatform && hitBelow) {
            DebugHit = true;
            _platformNormal = hit.normal;
        } else if (hitAbove) {
            _ceilingNormal = hit.normal;
        }
    }

    public void Move(float horizontal, float vertical, bool jump) {
        _offset = Vector3.zero;
        _forward = new Vector3(Pivot.transform.forward.x, 0f, Pivot.transform.forward.z).normalized;
        _right = new Vector3(Pivot.right.x, 0f, Pivot.right.z).normalized;
        
        _runVelocity = RunSpeed * (horizontal * _right + vertical * _forward);
        
        DebugHit = false;
        
        if (_characterController.isGrounded) {
            _platformNormal.Normalize();
            _runVelocity = Vector3.ProjectOnPlane(_runVelocity, _platformNormal);

            _platformSlope = Vector3.Angle(_platformNormal, transform.up);
            _downhill = Vector3.Cross(_platformNormal, Vector3.Cross(_platformNormal, transform.up)).normalized;

            // Check if platform is steep enough to slide down
            if (_platformSlope >= SlideAngle) {
                _slideVelocity += _downhill * Gravity; //Vector3.Dot(-Vector3.up * Gravity, _downhill);
            } else {
                // Cancel sliding on shallow inclines
                _slideVelocity = Vector3.zero;
            }

            bool isHitAbove = (_characterController.collisionFlags & CollisionFlags.Above) != 0;

            // Jump
            if (jump && !_isJumping) {
                _jumpVelocity = transform.up * JumpSpeed;
                _isJumping = true;
            } else if (_isJumping && isHitAbove) {
                _ceilingSlope = Vector3.Angle(_ceilingNormal, -transform.up);
                if (_ceilingSlope < CeilingBumpAngle) {
                    // Cancel the jump velocity if the player hits their head on a ceiling
                    //_jumpVelocity = Vector3.zero;
                } else {
                    // Slide along the wall
                }
                //_jumpVelocity = Vector3.ProjectOnPlane(_jumpVelocity, _ceilingNormal).normalized * _jumpVelocity.magnitude;
            } else {
                // Attach Player to platform
                bool platformFound = _platformDetector.CheckForPlatforms(out _platformOffset);

                if (platformFound) {
                    _offset += _platformOffset;
                }

                _jumpVelocity = Vector3.zero;
                _isJumping = false;

                // Even while the player is grounded, slightly push it into the ground to trigger the collision event and properly update CharacterController.isGrounded
                // TODO: Replace this with an internal isGrounded
                _gravityVelocity = Gravity * -Vector3.up;
            }
        } else {
            _slideVelocity = Vector3.zero;
            _gravityVelocity += Gravity * -Vector3.up;
        }
        
        _velocity = Time.deltaTime * (_runVelocity + _jumpVelocity + _gravityVelocity + _slideVelocity) + _offset;
        _characterController.Move(_velocity);

        DebugIsGrounded = _characterController.isGrounded;
        DebugCollidedSides = (_characterController.collisionFlags & CollisionFlags.CollidedSides) != 0;
        DebugCollidedBottom = (_characterController.collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _forward);
        Gizmos.DrawLine(transform.position, transform.position + _right);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + _platformNormal);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + _downhill);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + _ceilingNormal);
    }
}
