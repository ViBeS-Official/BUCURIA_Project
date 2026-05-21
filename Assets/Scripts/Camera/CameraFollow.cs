using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private PlayerMovement _movement;

    [Header("Settings")]
    public Vector3 _menuOffset = new(2, 1, -1);
    public Vector3 _normalOffset = new(0, 2, -2);
    public Vector3 _slideOffset = new(0, 1, -2);
    public float _smoothSpeed = 5f;
    public Transform _targetFollow;

    void Start()
    {
        _movement = GameManager.Instance.GetPlayer.GetComponentInChildren<PlayerMovement>();
    }

    void LateUpdate()
    {
        if (GameManager.Instance && GameManager.Instance.GetPlayer == null) return;
        Vector3 desiredPosition = GameManager.Instance.GetPlayer.GetPlayerTransform.position + (GameManager.Instance.gameState == GameState.Menu ? _menuOffset : (_movement && _movement.IsSlide ? _slideOffset : _normalOffset));
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        transform.LookAt(_targetFollow);
    }
}