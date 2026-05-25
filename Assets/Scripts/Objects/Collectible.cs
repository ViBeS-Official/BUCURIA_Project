using UnityEngine;

public class Collectible : MonoBehaviour
{
    private MeshGenerator _meshGenerator;

    [Header("Score")]
    public int value = 1;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 30f;

    [Header("Floating")]
    [SerializeField] private float _floatSpeed = 1f;
    [SerializeField] private float _floatHeight = 0.25f;

    private Vector3 _startPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_meshGenerator)
            {
                UIManager.Instance?.AddScore(_meshGenerator.GetCollectibleType, value);
                if (_meshGenerator.GetCollectibleType == CollectibleType.Candy) GameInventory.Instance.AddCandy(_meshGenerator.GetCandyName);
            }
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _startPosition = transform.position;
        _meshGenerator = GetComponent<MeshGenerator>();
    }

    private void Update()
    {
        Rotate();
        Float();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.World);
    }

    private void Float()
    {
        float yOffset = Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;
        Vector3 newPosition = _startPosition;
        newPosition.y += yOffset;
        transform.position = newPosition;
    }
}