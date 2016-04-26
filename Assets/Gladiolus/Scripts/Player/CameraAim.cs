using UnityEngine;
using System.Collections;

public class CameraAim : MonoBehaviour {

    public Transform ViewTransform;
    public Transform AimingTransform;
    public LayerMask AimLayers;
    public bool IntersectionFound;

    private Vector3 _hitPoint;
    private Vector3 _direction;

	// Use this for initialization
	void Awake () {
        
	}
	
	// Update is called once per frame
	void Update () {
        // Get the plane created by the camera's direction and this object's position
        Plane castStartPlane = new Plane(ViewTransform.forward, transform.position);

        // Find the intersection of that plane with the Camera's forward ray
        Ray viewForwardRay = new Ray(ViewTransform.position, ViewTransform.forward);
        float rayDistance;
        castStartPlane.Raycast(viewForwardRay, out rayDistance);

        // Use that intersection as the start of the aiming ray
        Vector3 startCastPoint = viewForwardRay.GetPoint(rayDistance);

        // Cast from the found starting point along the forward direction of the camera
        RaycastHit hitInfo;
        if (IntersectionFound = Physics.Raycast(startCastPoint, ViewTransform.forward, out hitInfo)) {
            // Make this object rotate its aiming transform towards the point of contact
            _hitPoint = hitInfo.point;
            _direction = _hitPoint - AimingTransform.position;

            AimingTransform.rotation = Quaternion.LookRotation(_direction, AimingTransform.up);
        } else {
            AimingTransform.rotation = ViewTransform.rotation;
        }
	}

    void OnDrawGizmos() {
        if (IntersectionFound) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_hitPoint, 0.25f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(ViewTransform.position, 100f * ViewTransform.forward);
    }
}
