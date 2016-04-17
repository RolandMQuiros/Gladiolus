using UnityEngine;
using System.Collections;

public class CameraAim : MonoBehaviour {

    public Transform ViewTransform;
    public Transform AimingTransform;
    public LayerMask AimLayers;

    private Vector3 _hitPoint;
    private Vector3 _direction;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hitInfo;
        if (Physics.Raycast(ViewTransform.position, ViewTransform.forward, out hitInfo)) {
            _hitPoint = hitInfo.point;
            _direction = _hitPoint - AimingTransform.position;

            AimingTransform.rotation = Quaternion.LookRotation(_direction, AimingTransform.up);
        } else {
            AimingTransform.rotation = ViewTransform.rotation;
        }
	}

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ViewTransform.position, _hitPoint);
    }
}
