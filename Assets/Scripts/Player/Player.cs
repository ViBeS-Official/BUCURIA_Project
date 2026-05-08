using UnityEngine;

public interface IPlayer
{
    void Initialize(Player player);
}

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private Transform _transform;
    private CharacterController _controller;
    private IPlayer[] _playerScripts;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _controller = GetComponent<CharacterController>();
        _playerScripts = GetComponentsInChildren<IPlayer>();
        for (int i = 0; i < _playerScripts.Length; i++) _playerScripts[i].Initialize(this);
    }

    public void SetPosition(Vector3 pos)
    {
        _controller.enabled = false;
        _transform.position = pos;
        _controller.enabled = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Obstacle"))
        {
            GameManager.Instance?.GameOver();
        }
    }

    public Transform GetPlayerTransform => _transform;
    public CharacterController GetPlayerController => _controller;
}