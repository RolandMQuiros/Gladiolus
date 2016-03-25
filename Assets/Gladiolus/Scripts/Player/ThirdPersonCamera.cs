using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
    private const string INPUT_VIEW_X = "View X";
    private const string INPUT_VIEW_Y = "View Y";

    public Transform pivot;

    public float SensitivityX = 100f;
    public float SensitivityY = 100f;
    public float VerticalLimit = 60f;

    private float _mouseDeltaX;
    private float _mouseDeltaY;

    private float _rotationX;
    private float _rotationY;

    void Start() {
        //Cursor.visible = false;
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

            pivot.rotation = Quaternion.Euler(_rotationY, _rotationX, 0f);
        }
	}

    void OnDrawGizmos() {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(pivot.position, pivot.position + pivot.forward);
    }
}
