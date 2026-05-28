using System.Collections;
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

    private Quaternion _yRotation;
    public Quaternion _surfaceRotation;

    [Header("Slide")]
    public float _slideTime = 1f;
    public Vector3 _slideCenter = new(0f, 0.5f, 0f);
    public float _slideHeight = 1f;
    private bool _isSliding;
    private bool _canStopSlide;
    private bool _buttonUp;
    private Coroutine _slideCoroutine;

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

    void Start()
    {
        GameManager.Instance.OnGameOver += StopSlide;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= StopSlide;
    }

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
        if (_canStopSlide && _buttonUp) StopSlide();
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartSlide();
            _buttonUp = false;
        }
        if (Input.GetKeyUp(KeyCode.S)) _buttonUp = true;
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
        if (!GameManager.Instance.IsGameStart) return;
        float targetYRotation = _horizontalInput * _rotationAngle;
        _yRotation = Quaternion.Euler(0f, targetYRotation, 0f);
        ApplyRotation();
    }
    private void ApplyRotation()
    {
        Quaternion final = _surfaceRotation * _yRotation;
        _player.GetMeshTransform.rotation = Quaternion.Slerp(_player.GetMeshTransform.rotation, final, _rotationSmooth * Time.deltaTime);
    }

    private void StartSlide()
    {
        if (!GameManager.Instance.IsGameStart || _isSliding) return;
        _isSliding = true;
        _gravity.Slide();
        _player.GetPlayerController.center = _slideCenter;
        _player.GetPlayerController.height = _slideHeight;
        _player.GetAnimator.SetBool("IsSlide", true);
        PlayerStatsSystem.Instance?.AddSlide();
        AudioManager.Instance?.Play("Slide");
        if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
        _slideCoroutine = StartCoroutine(SlideRoutine());
    }
    private IEnumerator SlideRoutine()
    {
        _canStopSlide = false;
        yield return new WaitForSeconds(_slideTime);
        _canStopSlide = true;
        //StopSlide();
    }
    public void StopSlide()
    {
        if (!_isSliding) return;
        if (_slideCoroutine != null)
        {
            StopCoroutine(_slideCoroutine);
            _slideCoroutine = null;
        }
        _player.GetPlayerController.center = _normalCenter;
        _player.GetPlayerController.height = _normalHeight;
        _player.GetAnimator.SetBool("IsSlide", false);
        AudioManager.Instance?.Stop("Slide");
        _isSliding = false;
    }

    public bool IsSlide => _isSliding;
}