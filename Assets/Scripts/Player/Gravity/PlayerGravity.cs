using UnityEngine;

public class PlayerGravity : MonoBehaviour, IPlayer
{
    private Player _player;
    private PlayerMovement _movement;

    [Header("Gravity / Jump")]
    public float _gravity = -10f;
    public float _jumpForce = 8f;

    private Vector3 _velocity;

    public void Initialize(Player player)
    {
        _player = player;
        _movement = _player.GetComponentInChildren<PlayerMovement>();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPause || !_player) return;
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
        if (GameManager.Instance.IsGameStart && _player.GetPlayerController.isGrounded)
        {
            _velocity.y = _jumpForce;
            _movement.StopSlide();
            _player.GetAnimator.SetTrigger("Jump");
        }
    }

    public Vector3 GetVelocity() => _velocity;
    public bool IsJumping => !_player.GetPlayerController.isGrounded;
}