using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
    private const string INPUT_VIEW_X = "View X";
    private const string INPUT_VIEW_Y = "View Y";

    public Transform Pivot;
    public Transform Camera;

    public float SensitivityX = 100f;
    public float SensitivityY = 100f;
    public float VerticalLimit = 60f;
    public float ObstructionPadding = 0.01f;

    public LayerMask ObstructionLayer;

    private float _mouseDeltaX;
    private float _mouseDeltaY;

    private float _rotationX;
    private float _rotationY;

    private Vector3 _initialOffset;
    private Vector3 _castPosition;
    private Vector3 _obstructionPoint;

    void Start() {
        //Cursor.visible = false;
        _initialOffset = Camera.localPosition;
    }

	// Update is called once per frame
	void Update () {
        //Cursor.lockState = CursorLockMode.Locked;
        // Rotate the camera using mouse input
        _mouseDeltaX = Input.GetAxis(INPUT_VIEW_X) * SensitivityX * Time.deltaTime;
        _mouseDeltaY = Input.GetAxis(INPUT_VIEW_Y) * SensitivityY * Time.deltaTime;
        
        if (_mouseDeltaX != 0f || _mouseDeltaY != 0f) {
            _rotationX += _mouseDeltaX;
            _rotationY -= _mouseDeltaY;
            _rotationY = Mathf.Clamp(_rotationY, -VerticalLimit, VerticalLimit);

            Pivot.rotation = Quaternion.Euler(_rotationY, _rotationX, 0f);
        }

        RaycastHit hitInfo;
        _castPosition = Pivot.TransformPoint(_initialOffset);
        if (Physics.Linecast(Pivot.position, _castPosition, out hitInfo, ObstructionLayer.value)) {
            Camera.position = hitInfo.point;
            _obstructionPoint = hitInfo.point;
            Debug.Log("hit!");
        } else {
            Camera.localPosition = _initialOffset;
            _obstructionPoint = Camera.position;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Pivot.position, _castPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_obstructionPoint, 0.1f);
    }
}
