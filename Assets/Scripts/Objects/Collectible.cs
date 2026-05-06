using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player) player.AddScore(value);
            Destroy(gameObject);
        }
    }
}