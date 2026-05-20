using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;

    public bool _gameOver;
    public event Action OnRestart;

    private void Awake() => Instance = this;

    void Start() => GameStart();

    public void GameStart()
    {
        _gameOver = false;
        player.GetAnimator.SetBool("IsRun", true);
        player.GetAnimator.SetTrigger("GameStart");
        UIManager.Instance?.GameStart();
        if (player) player.SetPosition(Vector3.zero);
        OnRestart?.Invoke();
    }

    public void GameOver()
    {
        if (_gameOver) return;
        _gameOver = true;
        player.GetAnimator.SetTrigger("GameOver");
        UIManager.Instance?.GameOver();
    }

    public Player GetPlayer => player;
}