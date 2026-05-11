using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;

    public bool gameOver;
    public event Action OnRestart;

    private void Awake() => Instance = this;

    void Start() => GameStart();

    public void GameStart()
    {
        gameOver = false;
        UIManager.Instance?.GameStart();
        if (player) player.SetPosition(Vector3.zero);
        OnRestart?.Invoke();
    }

    public void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        UIManager.Instance?.GameOver();
    }
}