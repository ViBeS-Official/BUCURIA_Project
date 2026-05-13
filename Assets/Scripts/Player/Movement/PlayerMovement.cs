using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPlayer
{
    private Player _player;

    [Header("Movement")]
    public float _forwardSpeed = 10f;
    public float _sideSpeed = 5f;
    public float _maxForwardSpeed = 20f;
    public float _maxSideSpeed = 10f;

    [Header("Difficulty")]
    [Tooltip("Насколько быстро увеличивается сложность")] public float _distanceDifficultyMultiplier = 0.02f;
    [Tooltip("Через сколько метров увеличивается сложность")] public float _difficultyDistanceStep = 50f;

    private float _currentForwardSpeed;
    private float _currentSideSpeed;

    [Header("Smooth")]
    public bool _canSmooth = true;
    public float _smoothSpeed = 5;

    private float _horizontalInput;

    public void Initialize(Player player)
    {
        _player = player;

        _currentForwardSpeed = _forwardSpeed;
        _currentSideSpeed = _sideSpeed;
    }

    private void Update()
    {
        if (!_player || GameManager.Instance.gameOver) return;
        HandleDifficulty();
        HandleInput();
        HandleMovement();
    }

    private void HandleDifficulty()
    {
        float distanceTravelled = transform.position.z;
        float difficultyLevel = Mathf.FloorToInt(distanceTravelled / _difficultyDistanceStep);
        float difficultyMultiplier = 1f + (difficultyLevel * _distanceDifficultyMultiplier);
        _currentForwardSpeed = Mathf.Min(_forwardSpeed * difficultyMultiplier, _maxForwardSpeed);
        _currentSideSpeed = Mathf.Min(_sideSpeed * difficultyMultiplier, _maxSideSpeed);
    }

    private void HandleInput()
    {
        if (_canSmooth) _horizontalInput = Mathf.Lerp(_horizontalInput, Input.GetAxis("Horizontal"), _smoothSpeed * Time.deltaTime);
        else _horizontalInput = Input.GetAxis("Horizontal");
    }
    private void HandleMovement()
    {
        Vector3 move = Vector3.forward * _currentForwardSpeed;
        move += Vector3.right * _horizontalInput * _currentSideSpeed;
        _player.GetPlayerController.Move(move * Time.deltaTime);
    }
}