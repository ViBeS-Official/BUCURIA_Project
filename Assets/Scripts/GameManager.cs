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
        player.GetAnimator.SetBool("IsRun", true);
        player.GetAnimator.SetTrigger("GameStart");
        UIManager.Instance?.GameStart();
        if (player) player.SetPosition(Vector3.zero);
        OnRestart?.Invoke();
    }

    public void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        player.GetAnimator.SetTrigger("GameOver");
        UIManager.Instance?.GameOver();
    }

    public Player GetPlayer => player;
}