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
    [SerializeField] private float _rotationAngle = 25f;
    [SerializeField] private float _rotationSmooth = 10f;

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
        if (!_player) return;
        HandleDifficulty();
        HandleInput();
        HandleMovement();
        HandleVisualRotation();
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
        if (!GameManager.Instance.IsGameStart)
        {
            _horizontalInput = 0f;
            return;
        }
        if (_canSmooth) _horizontalInput = Mathf.Lerp(_horizontalInput, Input.GetAxis("Horizontal"), _smoothSpeed * Time.deltaTime);
        else _horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.S)) StartSlide();
    }
    private void HandleMovement()
    {
        Vector3 move = Vector3.forward * _currentForwardSpeed;
        move += Vector3.right * _horizontalInput * _currentSideSpeed;
        if (!GameManager.Instance.IsGameStart)
        {
            move.x = 0f;
            move.z = 0f;
        }
        if (!GameManager.Instance.IsPause)
        {
            move += _gravity.GetVelocity();
            _player.GetPlayerController.Move(move * Time.deltaTime);
        }
    }
    private void HandleVisualRotation()
    {
        if (GameManager.Instance.IsPause) return;
        float targetYRotation = _horizontalInput * _rotationAngle;
        Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
        _player.GetMeshTransform.localRotation = Quaternion.Lerp(_player.GetMeshTransform.localRotation, targetRotation, _rotationSmooth * Time.deltaTime);
    }

    private void StartSlide()
    {
        if (GameManager.Instance.IsGameStart && !_isSliding && !_gravity.IsJumping)
        {
            _isSliding = true;
            _player.GetPlayerController.center = _slideCenter;
            _player.GetPlayerController.height = _slideHeight;
            _player.GetAnimator.SetBool("IsSlide", true);
            Invoke(nameof(StopSlide), _slideTime);
        }
    }
    public void StopSlide()
    {
        _player.GetPlayerController.center = _normalCenter;
        _player.GetPlayerController.height = _normalHeight;
        _isSliding = false;
        _player.GetAnimator.SetBool("IsSlide", false);
    }
    public bool IsSlide => _isSliding;
}