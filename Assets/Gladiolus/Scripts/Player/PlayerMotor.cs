using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour {
    public Transform Pivot;

    public float RunSpeed = 10f;
    public float WalkSpeed = 10f;
    public float JumpSpeed = 100f;
    public float Gravity = 9.81f;
    public int SlideAngle = 60;
    public int CeilingBumpAngle = 25;

    public LayerMask PlatformLayers;

    //[HideInInspector]
    public bool IsJumping;

    private CharacterController _characterController;

    private Vector3 _forward;
    private Vector3 _right;

    public Vector3 _velocity;
    public Vector3 _offset;
    
    public Vector3 _gravityVelocity;

    /// <summary>The normal of any Platform the character controller collides with</summary>
    public Vector3 _platformNormal;
    /// <summary>The normal of platforms that collide with the bottom of the character capsule.  Used for sliding the character off steep inclines.</summary>
    public Vector3 _floorNormal;
    /// <summary>The normal of platforms that collide with the top of the character capsule.  Used to slide the character off shallow ceiling angles when moving upward.</summary>
    public Vector3 _ceilingNormal;
    /// <summary>Unit vector pointing down the plane the character stands on.  This is the direction the character will slide down if the incline is past a steepness threshold.</summary>
    public Vector3 _downhill;
    /// <summary>The angle of the floor, in degrees</summary>
    public float _floorAngle;
    /// <summary>The angle of the ceiling, in degrees</summary>
    public float _ceilingAngle;

    /// <summary>Whether or not the top of the character controller is in contact with an object in the PlatformLayers mask</summary>
    public bool IsTouchingCeiling = false;
    /// <summary>Whether or not the bottom of the character controller is in contact with an object in the PlatformLayers mask</summary>
    public bool IsTouchingFloor = false;
    /// <summary>Whether or not the side of the character controller is in contact with an object in the PlatformLayers mask</summary>
    public bool IsTouchingWall = false;

    /// <summary>When the controller hits a ceiling, this flags whether or not it already canceled any upward velocity.  Makes sure the cancellation only happens in a single frame.</summary>
    public bool _didCeilingCancelJump = false;
    public bool _isTouchingPlatform = false;

    private Color _characterSkinGizmoColor = new Color(0.2f, 0.8f, 0.3f);

	void OnEnable() {
        _characterController = GetComponent<CharacterController>();
	}

    void Awake() {
        _characterController = GetComponent<CharacterController>();
    }
    
    // TODO: Move this to PlayerController
    void Update() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");

        Move(horizontal, vertical, jump);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        // Note: CollisionFlags aren't updated at the point OnControllerColliderHit is called, so
        // assigning the IsTouching variables here leads to inconsistent behavior
        _isTouchingPlatform = (PlatformLayers.value & (1 << hit.gameObject.layer)) != 0;
        if (_isTouchingPlatform) {
            _platformNormal = hit.normal;
        }
    }

    public void Move(float horizontal, float vertical, bool jump) {
        // These two vectors are the only things that aren't based on transform.up. Fix this maybe?
        _forward = new Vector3(Pivot.transform.forward.x, 0f, Pivot.transform.forward.z).normalized;
        _right = new Vector3(Pivot.right.x, 0f, Pivot.right.z).normalized;
        _offset = Vector3.zero;

        Vector3 motionVelocity = RunSpeed * (_forward * vertical + _right * horizontal);

        if (_characterController.isGrounded) {

            // The floor is greater than a given angle, slide down it
            _floorAngle = Vector3.Angle(_floorNormal, transform.up);
            if (_floorAngle > SlideAngle) {
                _gravityVelocity += Gravity * _downhill;
            // If not sliding, allow the player to jump
            } else if (jump) {
                _gravityVelocity = JumpSpeed * transform.up;
                IsJumping = true;
            // If neither sliding nor jumping, cancel gravity entirely
            } else {
                _gravityVelocity = Vector3.zero;
                _offset = -_characterController.stepOffset * transform.up; // Push controller into ground to trigger proper isGrounded behavior
                IsJumping = false;
            }

        } else {
            // If the top of the controller hits a platform, cancel its upward velocity
            float upwardSpeed = Vector3.Dot(motionVelocity + _gravityVelocity, transform.up);
            if (IsTouchingCeiling && upwardSpeed > 0f) {
                _ceilingAngle = Vector3.Angle(_ceilingNormal, -transform.up);
                // Check if we aleady canceled the upward speed.  This makes up for the CollisionFlags.Above being true for more than one frame.
                if (_ceilingAngle < CeilingBumpAngle) {
                    _gravityVelocity = Vector3.zero;
                    _offset = -_characterController.stepOffset * transform.up;
                }
            }

            _gravityVelocity -= Gravity * transform.up;
        }

        _velocity = motionVelocity + _gravityVelocity;

        bool isHit = _characterController.collisionFlags != 0;
        if (isHit) {
            _velocity = Vector3.ProjectOnPlane(_velocity, _platformNormal);
        }

        _characterController.Move(_velocity * Time.deltaTime + _offset);

        if (_isTouchingPlatform) {
            IsTouchingFloor = (_characterController.collisionFlags & CollisionFlags.Below) != 0;
            IsTouchingCeiling = (_characterController.collisionFlags & CollisionFlags.Above) != 0;
            IsTouchingWall = (_characterController.collisionFlags & CollisionFlags.Sides) != 0;

            if (IsTouchingFloor) {
                _floorNormal = _platformNormal;
                _downhill = Vector3.Cross(_floorNormal, Vector3.Cross(_floorNormal, transform.up)).normalized;
            } else if (IsTouchingCeiling) {
                _ceilingNormal = _platformNormal;
            }
        }
    }

    void OnDrawGizmos() {
        Vector3 feet = transform.position - (_characterController.height / 2f) * Vector3.up;
        Vector3 head = transform.position + (_characterController.height / 2f) * Vector3.up;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(feet, feet + _floorNormal);
        Gizmos.DrawLine(head, head + _ceilingNormal);
        Gizmos.DrawLine(transform.position, transform.position + _platformNormal);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + _velocity);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(feet, feet + _downhill);

        Gizmos.color = _characterSkinGizmoColor;
        float skinRadius = _characterController.radius + _characterController.skinWidth;
        Gizmos.DrawWireSphere(transform.position + (_characterController.height / 2f - _characterController.radius) * Vector3.up, skinRadius);
        Gizmos.DrawWireSphere(transform.position - (_characterController.height / 2f - _characterController.radius) * Vector3.up, skinRadius);
    }
}
