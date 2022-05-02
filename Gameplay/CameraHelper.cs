using UnityEngine;

public static class CameraHelper
{
    public static void CameraFocus(Transform target)
    {
        Camera.main.orthographicSize = 1.05f;
        Camera.main.transform.LookAt(target.position);
    }
}
