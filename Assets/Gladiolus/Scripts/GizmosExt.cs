using UnityEngine;

public class GizmosExt {
    private const int CYLINDER_SEGMENTS = 4;

    public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius) {
        Color oldHandleColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Gizmos.color;

        UnityEditor.Handles.DrawWireDisc(center, normal, radius);

        UnityEditor.Handles.color = oldHandleColor;
    }

    public static void DrawWireCylinder(Vector3 start, Vector3 end, float radius) {
        Vector3 direction = start - end;
        Vector3 basisA = new Vector3();
        Vector3 basisB = new Vector3();

        Vector3.OrthoNormalize(ref direction, ref basisA, ref basisB);

        Vector3 offset = new Vector3();

        float angle = 0f;
        for (int i = 0; i < CYLINDER_SEGMENTS; i++) {
            angle += 2f * Mathf.PI / CYLINDER_SEGMENTS;
            offset = radius * Mathf.Sin(angle) * basisA + radius * Mathf.Cos(angle) * basisB;
            Gizmos.DrawLine(start + offset, end + offset);
        }

        Color oldHandleColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Gizmos.color;

        UnityEditor.Handles.DrawWireDisc(start, direction, radius);
        UnityEditor.Handles.DrawWireDisc(end, direction, radius);

        UnityEditor.Handles.color = oldHandleColor;
    }
}
