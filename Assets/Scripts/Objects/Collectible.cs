using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Score")]
    public int value = 1;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f;

    [Header("Floating")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float floatHeight = 0.25f;

    private Vector3 startPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance?.AddScore(value);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        Rotate();
        Float();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    private void Float()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        Vector3 newPosition = startPosition;
        newPosition.y += yOffset;
        transform.position = newPosition;
    }
}