using UnityEngine;

public class PlayerGravity : MonoBehaviour, IPlayer
{
    private Player _player;

    [Header("Gravity / Jump")]
    public float _gravity = -10f;
    public float _jumpForce = 8f;

    private Vector3 _velocity;

    public void Initialize(Player player) => _player = player;

    private void Update()
    {
        if (!_player || GameManager.Instance.gameOver) return;
        HandleInput();
        HandleMovement();
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W)) Jump();
    }
    private void HandleMovement()
    {
        if (_player.GetPlayerController.isGrounded && _velocity.y < 0) _velocity.y = -2f;
        _velocity.y += _gravity * Time.deltaTime;
    }

    private void Jump()
    {
        if (_player.GetPlayerController.isGrounded) _velocity.y = _jumpForce;
    }

    public Vector3 GetVelocity() => _velocity;
}