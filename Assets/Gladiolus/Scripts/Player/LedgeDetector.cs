using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class LedgeDetector : MonoBehaviour {
    private static Color _ledgeFinderGizmoColor = new Color(0.5f, 0f, 0.5f);

    /// <summary>To detect ledges, the detector performs a cylinder cast from this point, down to this GameObject's position.</summary>
    [Tooltip("To detect ledges, the detector performs a cylinder cast from this point, down to this GameObject's position.")]
    public float LedgeCastHeight = 10f;
    /// <summary>The radius of the ledge detection cylinder cast</summary>
    [Tooltip("The radius of the ledge detection cylinder cast")]
    public float LedgeCastRadius = 2f;
    /// <summary>Ledge grabbing distance</summary>
    public float GrabbingDistance = 1f;
    /// <summary>Collision layers to check for ledges</summary>
    [Tooltip("Collision layers to check for ledges")]
    public LayerMask PlatformLayers;
    /// <summary>Angle, in degrees, between the collided platform normal and this transform's up vector.  If greater than this angle, the collided platform is a wall.</summary>
    [Tooltip("Angle, in degrees, between the collided platform normal and this transform's up vector.  If greater than this angle, the collided platform is a wall.")]
    public float WallAngleStart = 45;
    public float WallAngleEnd = 135;

    public float LedgeAngleThreshold = 45;

    public bool WasWallFound;
    /// <summary>Whether or not a grabbable ledge was found</summary>
    public bool WasLedgeFound;
    /// <summary>Position on ledge 
    ///  </summary>
    public Vector3 LedgePoint;
    
    private SphereCollider _wallFinder;
    private Plane _wallPlane = new Plane();
    private Plane _ledgePlane = new Plane();
    private Vector3 _ledgePoint;
    private Vector3 _ledgeNormal;
    private Transform _wallTransform;
    public float _ledgeDistance;

    public float _wallAngle;
    public float _ledgeAngle;

    private Ray wallFinderRay;

	void Awake () {
        _wallFinder = GetComponent<SphereCollider>();
	}

    void OnTriggerStay(Collider other) {
        // Get wall plane
        RaycastHit hitInfo;
        Vector3 direction = (other.transform.position - transform.position).normalized;
        wallFinderRay = new Ray(transform.position, direction);

        WasWallFound = false;
        WasLedgeFound = false;

        if (other.Raycast(wallFinderRay, out hitInfo, _wallFinder.radius)) {
            // Check if we're colliding with a wall
            _wallAngle = Vector3.Angle(hitInfo.normal, transform.up);
            _wallPlane.SetNormalAndPosition(hitInfo.normal, hitInfo.point);

            if (WasWallFound = _wallAngle > WallAngleStart && _wallAngle < WallAngleEnd) {
                _wallTransform = other.transform;

                // TODO: get the wall plane, get the ledge plane, find the line intersection as the ledge

                // Cast from ledge cast point and find ledge line
                Vector3 start = transform.position + LedgeCastHeight * transform.up;
                if (Physics.SphereCast(start, LedgeCastRadius, -transform.up, out hitInfo, PlatformLayers)) {
                    _ledgePlane.SetNormalAndPosition(hitInfo.normal, hitInfo.point);
                    _ledgePoint = hitInfo.point;
                    _ledgeNormal = hitInfo.normal;
                    _ledgeDistance = (hitInfo.point - transform.position).magnitude;
                    _ledgeAngle = Vector3.Angle(_ledgePlane.normal, transform.up);

                    if (WasLedgeFound = (_ledgeDistance < GrabbingDistance && _ledgeAngle < LedgeAngleThreshold)) {
                        
                    }
                }
            }
        }    
    }

	// Update is called once per frame
	void Update () {
	}

    void OnDrawGizmos() {
        Gizmos.color = _ledgeFinderGizmoColor;

        Gizmos.DrawLine(transform.position, transform.position + _wallPlane.normal);
        Gizmos.DrawLine(wallFinderRay.origin, wallFinderRay.origin + 100f * wallFinderRay.direction);

        //if (WasWallFound) {
            GizmosExt.DrawWireCylinder(transform.position + LedgeCastHeight * transform.up, transform.position, LedgeCastRadius);
            GizmosExt.DrawWireDisc(_ledgePoint, _ledgeNormal, LedgeCastRadius);
        //}
    }
}
