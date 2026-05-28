using UnityEngine;

public class PlayerSurfaceRotation : MonoBehaviour, IPlayer
{
    private Player _player;
    private PlayerMovement _movement;

    [Header("Raycast")]
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float raycastOffsetY = 1f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    private Vector3 _currentNormal = Vector3.up;

    public void Initialize(Player player)
    {
        _player = player;
        _movement = player.GetComponentInChildren<PlayerMovement>();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameStart) return;
        RotateToSurface();
    }

    private void RotateToSurface()
    {
        Quaternion targetRotation;
        if (_movement.IsSlide)
        {
            Vector3 origin = transform.position + Vector3.up * raycastOffsetY;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayDistance, groundMask)) _currentNormal = hit.normal;
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, _currentNormal).normalized;
            if (forward.sqrMagnitude < 0.001f) forward = transform.forward;
            targetRotation = Quaternion.LookRotation(forward, _currentNormal);
        }
        else targetRotation = Quaternion.identity;
        _movement._surfaceRotation = Quaternion.Slerp(_movement._surfaceRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}