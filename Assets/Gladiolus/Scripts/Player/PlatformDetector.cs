using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Keeps track of platforms beneath a non-rigid body object.  Accounts for moving platforms.
/// Does not yet account for rotations or scales.
/// </summary>
public class PlatformDetector : MonoBehaviour {
    public Vector3 Start;
    public Vector3 End = new Vector3(0f, -1f, 0f);
    public LayerMask Layer;
    
    private bool _wasPlatformFound;
    public bool WasPlatformFound {
        get { return _wasPlatformFound; }
    }

    private Vector3 _offset;
    public Vector3 Offset {
        get { return _offset; }
    }

    public string DebugCast;
    public Vector3 DebugOffset;

    private GameObject _currentPlatform;
    private Vector3 _prevPosition;

    public bool CheckForPlatforms(out Vector3 offset) {
        RaycastHit hitInfo;
        _offset = Vector3.zero;

        _wasPlatformFound = Physics.Linecast(
            transform.position + Start,
            transform.position + End,
            out hitInfo,
            Layer.value
        );

        if (_wasPlatformFound) {
            // If this is the same platform from the previous frame, transform this object
            // to match it
            if (hitInfo.collider.gameObject == _currentPlatform) {
                // Offset to contact ground
                _offset = hitInfo.point - transform.position;

                // Calculate translation offset
                _offset += _currentPlatform.transform.position - _prevPosition;
                _prevPosition = _currentPlatform.transform.position;
            } else {
                _currentPlatform = hitInfo.collider.gameObject;
                _prevPosition = _currentPlatform.transform.position;
            }

            DebugCast = hitInfo.collider.gameObject.name;
        } else {
            DebugCast = _wasPlatformFound.ToString();
        }

        offset = _offset;

        return _wasPlatformFound;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Start, transform.position + End);
    }
}
