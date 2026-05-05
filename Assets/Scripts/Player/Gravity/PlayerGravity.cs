using UnityEngine;

public class PlayerGravity : MonoBehaviour, IPlayer
{
    private Player _player;

    [Header("Gravity")]
    public float _gravity = -10f;

    private Vector3 _velocity;

    public void Initialize(Player player) => _player = player;

    private void Update()
    {
        if (!_player) return;
        if (_player.GetPlayerController.isGrounded && _velocity.y < 0) _velocity.y = -2f;
        _velocity.y += _gravity * Time.deltaTime;
        _player.GetPlayerController.Move(_velocity * Time.deltaTime);
    }
}