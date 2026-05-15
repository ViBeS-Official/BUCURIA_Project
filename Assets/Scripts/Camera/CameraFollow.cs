using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 4, -8);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (GameManager.Instance && GameManager.Instance.GetPlayer == null) return;
        Vector3 desiredPosition = GameManager.Instance.GetPlayer.GetPlayerTransform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.LookAt(GameManager.Instance.GetPlayer.GetPlayerTransform);
    }
}