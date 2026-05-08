using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPlayer
{
    private Player _player;

    [Header("Movement")]
    public float _forwardSpeed = 10f;
    public float _sideSpeed = 5f;

    [Header("Smooth")]
    public bool _canSmooth = true;
    public float _smoothSpeed = 5;

    private float _horizontalInput;

    public void Initialize(Player player) => _player = player;

    private void Update()
    {
        if (!_player || GameManager.Instance.gameOver) return;
        if (_canSmooth) _horizontalInput = Input.GetAxis("Horizontal");
        else _horizontalInput = Mathf.Lerp(_horizontalInput, Input.GetAxis("Horizontal"), _smoothSpeed * Time.deltaTime);
        Vector3 move = Vector3.forward * _forwardSpeed;
        move += Vector3.right * _horizontalInput * _sideSpeed;
        _player.GetPlayerController.Move(move * Time.deltaTime);
    }
}