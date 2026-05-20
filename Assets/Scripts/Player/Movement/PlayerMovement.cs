using UnityEngine;

public class PlayerMovement : MonoBehaviour, IPlayer
{
    private Player _player;
    private PlayerGravity _gravity;

    [Header("Movement")]
    public float _forwardSpeed = 10f;
    public float _sideSpeed = 5f;
    public float _maxForwardSpeed = 20f;
    public float _maxSideSpeed = 10f;
    public Vector3 _normalCenter = new(0f, 0.8f, 0f);
    public float _normalHeight = 1.6f;

    [Header("Slide")]
    public float _slideTime = 1f;
    public Vector3 _slideCenter = new(0f, 0.5f, 0f);
    public float _slideHeight = 1f;
    private bool _isSliding;

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
        _gravity = _player.GetComponentInChildren<PlayerGravity>();

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
        float difficultyLevel = distanceTravelled / _difficultyDistanceStep;
        float difficultyMultiplier = 1f + (difficultyLevel * _distanceDifficultyMultiplier);
        _currentForwardSpeed = Mathf.Min(_forwardSpeed * difficultyMultiplier, _maxForwardSpeed);
        _currentSideSpeed = Mathf.Min(_sideSpeed * difficultyMultiplier, _maxSideSpeed);
    }

    private void HandleInput()
    {
        if (_canSmooth) _horizontalInput = Mathf.Lerp(_horizontalInput, Input.GetAxis("Horizontal"), _smoothSpeed * Time.deltaTime);
        else _horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.S)) StartSlide();
    }
    private void HandleMovement()
    {
        Vector3 move = Vector3.forward * _currentForwardSpeed;
        move += Vector3.right * _horizontalInput * _currentSideSpeed;
        move += _gravity.GetVelocity();
        _player.GetPlayerController.Move(move * Time.deltaTime);
    }

    private void StartSlide()
    {
        if (_isSliding) return;
        _isSliding = true;
        _player.GetPlayerController.height = _slideHeight;
        Invoke(nameof(StopSlide), _slideTime);
    }
    private void StopSlide()
    {
        _player.GetPlayerController.height = _normalHeight;
        _isSliding = false;
    }
}