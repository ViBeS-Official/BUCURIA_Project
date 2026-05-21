using System;
using UnityEngine;

public enum GameState
{
    Menu,
    GameStart,
    GameStop,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;
    public Vector3 _spawnPosition;
    public Vector3 _spawnRotation;
    public GameState gameState;
    public event Action OnMenu;
    public event Action OnStartGame;

    private void Awake() => Instance = this;

    private void Start() => Menu();

    public void GameStart()
    {
        gameState = GameState.GameStart;
        player.GetAnimator.SetBool("IsRun", true);
        player.GetAnimator.SetTrigger("GameStart");
        UIManager.Instance?.GameStart();
        if (player)
        {
            player.SetPosition(_spawnPosition);
            player.SetRotation(Quaternion.identity);
        }
        OnStartGame?.Invoke();
    }

    public void GameOver()
    {
        gameState = GameState.GameStop;
        player.GetAnimator.SetTrigger("GameOver");
        UIManager.Instance?.GameOver();
    }

    public void Menu()
    {
        gameState = GameState.Menu;
        player.GetAnimator.SetBool("IsRun", false);
        player.GetAnimator.SetTrigger("GameStart");
        UIManager.Instance?.Menu();
        if (player)
        {
            player.SetPosition(_spawnPosition);
            player.SetRotation(Quaternion.Euler(_spawnRotation));
        }
        OnMenu?.Invoke();
    }

    public Player GetPlayer => player;
    public bool IsGameStart => gameState == GameState.GameStart;
}