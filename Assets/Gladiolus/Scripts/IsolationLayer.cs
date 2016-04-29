using UnityEngine;
using System.Collections;

public static class IsolationLayer {
    public const int LAYER = 31;
    public const int MASK = (1 << LAYER);
    private static GameObject ms_isolatedObject;
    private static int ms_originalLayer;

    public static void Isolate(GameObject gameObject) {
        if (ms_isolatedObject) {
            ms_isolatedObject.layer = ms_originalLayer;
        }

        ms_originalLayer = gameObject.layer;
        ms_isolatedObject = gameObject;
        ms_isolatedObject.layer = LAYER;
    }

    public static void Clear() {
        if (ms_isolatedObject) {
            ms_isolatedObject.layer = ms_originalLayer;
        }

        ms_isolatedObject = null;
        ms_originalLayer = 0;
    }
}
