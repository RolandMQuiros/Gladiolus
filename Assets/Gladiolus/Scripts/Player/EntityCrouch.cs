using UnityEngine;
using System.Collections;

public class EntityCrouch : MonoBehaviour {
    public Transform Head;
    public float Padding = 0.01f;
    public Vector3 CrouchPosition;
    public LayerMask LayerMask;
    public float CrouchInterval = 1f;
    public bool AutomaticCrouch = true;
    public bool IsCrouching = false;

    public SphereCollider m_headCollider;
    public Vector3 m_startingPosition;
    public Vector3 m_direction;
    public float m_distanceToTopOfHead;

    void Start() {
        m_startingPosition = Head.localPosition;
        m_direction = m_startingPosition.normalized;

        m_headCollider = Head.gameObject.GetComponent<SphereCollider>();
        m_distanceToTopOfHead = m_startingPosition.magnitude + Vector3.Dot(m_headCollider.bounds.extents, m_direction) + Padding;
    }

    void FixedUpdate() {
        if (AutomaticCrouch) {
            Vector3 worldCrouchPosition = transform.TransformPoint(CrouchPosition);
            Ray crouchCast = new Ray(worldCrouchPosition, m_direction);

            RaycastHit hitInfo;
            if (Physics.SphereCast(crouchCast, m_headCollider.radius, out hitInfo, m_distanceToTopOfHead, LayerMask.value)) {
                CrouchInterval = ((hitInfo.point - worldCrouchPosition).magnitude + Padding) / m_distanceToTopOfHead;
            } else {
                CrouchInterval = 1f;
            }
        }

        Head.localPosition = CrouchPosition + (CrouchInterval * (m_startingPosition - CrouchPosition));
    }
    
}
