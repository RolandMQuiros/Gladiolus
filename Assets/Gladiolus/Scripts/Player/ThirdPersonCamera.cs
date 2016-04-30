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

    public LayerMask ObstructionMask;

    private float m_mouseDeltaX;
    private float m_mouseDeltaY;

    private float m_rotationX;
    private float m_rotationY;

    private Vector3 m_initialOffset;
    private Vector3 m_castPosition;
    private Vector3 m_obstructionPoint;

    void Start() {
        //Cursor.visible = false;
        m_initialOffset = Camera.localPosition;
    }

	// Update is called once per frame
	void Update () {
        //Cursor.lockState = CursorLockMode.Locked;
        // Rotate the camera using mouse input
        m_mouseDeltaX = Input.GetAxis(INPUT_VIEW_X) * SensitivityX * Time.deltaTime;
        m_mouseDeltaY = Input.GetAxis(INPUT_VIEW_Y) * SensitivityY * Time.deltaTime;
        
        if (m_mouseDeltaX != 0f || m_mouseDeltaY != 0f) {
            m_rotationX += m_mouseDeltaX;
            m_rotationY -= m_mouseDeltaY;
            m_rotationY = Mathf.Clamp(m_rotationY, -VerticalLimit, VerticalLimit);

            Pivot.rotation = Quaternion.Euler(m_rotationY, m_rotationX, 0f);
        }

        RaycastHit hitInfo;
        m_castPosition = Pivot.TransformPoint(m_initialOffset);
        if (Physics.Linecast(Pivot.position, m_castPosition, out hitInfo, ObstructionMask.value)) {
            Camera.position = hitInfo.point;
            m_obstructionPoint = hitInfo.point;
        } else {
            Camera.localPosition = m_initialOffset;
            m_obstructionPoint = Camera.position;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_obstructionPoint, 0.1f);
    }
}
