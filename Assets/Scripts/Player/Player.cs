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

    public int score = 0;
    public void AddScore(int amount)
    {
        score += amount;
        UIManager.Instance?.UpdateScore(score);
    }

    private void Start()
    {
        _transform = GetComponent<Transform>();
        _controller = GetComponent<CharacterController>();
        _playerScripts = GetComponentsInChildren<IPlayer>();
        for (int i = 0; i < _playerScripts.Length; i++) _playerScripts[i].Initialize(this);
    }

    public Transform GetPlayerTransform => _transform;
    public CharacterController GetPlayerController => _controller;
}