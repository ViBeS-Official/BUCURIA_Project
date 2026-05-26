using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCharacterRotate : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("Settings")]
    public float _rotationSpeed = 0.2f;
    public float _smooth = 20f;
    private float _targetRotation;
    private float _lastMouseX;

    void Start()
    {
        GameManager.Instance.OnStartGame += ResetRotation;
        _targetRotation = GameManager.Instance.GetSpawnRotationY;
    }
    private void OnDestroy() => GameManager.Instance.OnStartGame -= ResetRotation;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _lastMouseX = eventData.position.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - _lastMouseX;
        _targetRotation -= deltaX * _rotationSpeed;
        _lastMouseX = eventData.position.x;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsMenu) return;
        float currentY = GameManager.Instance.GetPlayer.GetPlayerTransform.eulerAngles.y;
        float newY = Mathf.LerpAngle(currentY, _targetRotation, Time.deltaTime * _smooth);
        GameManager.Instance.GetPlayer.GetPlayerTransform.rotation = Quaternion.Euler(0f, newY, 0f);
    }

    public void ResetRotation()
    {
        _targetRotation = GameManager.Instance.GetSpawnRotationY;
    }
}