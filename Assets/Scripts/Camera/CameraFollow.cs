using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 4, -8);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}