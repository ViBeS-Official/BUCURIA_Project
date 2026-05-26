using System;
using UnityEditor;
using UnityEngine;

public enum GameState
{
    Menu,
    GameStart,
    GameStop,
    Pause,
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
        if (gameState == GameState.GameStart) return;
        Pause(false);
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
        if (gameState == GameState.GameStop) return;
        Pause(false);
        gameState = GameState.GameStop;
        player.GetAnimator.SetTrigger("GameOver");
        UIManager.Instance?.GameOver();
    }

    public void Menu()
    {
        if (gameState == GameState.Menu) return;
        Pause(false);
        gameState = GameState.Menu;
        player.GetAnimator.SetBool("IsRun", false);
        player.GetAnimator.SetTrigger("GameStart");
        UIManager.Instance?.Menu();
        SetPlayerTransformOnSpawnTransform();
        OnMenu?.Invoke();
    }
    public void SetPlayerTransformOnSpawnTransform()
    {
        if (player)
        {
            player.SetPosition(_spawnPosition);
            player.SetRotation(Quaternion.Euler(_spawnRotation));
        }
    }

    public void Pause(bool active)
    {
        if (active) gameState = GameState.Pause;
        else gameState = GameState.GameStart;
        player.GetAnimator.speed = active ? 0f : 1f;
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    public Player GetPlayer => player;
    public float GetSpawnRotationY => _spawnRotation.y;
    public bool IsGameStart => gameState == GameState.GameStart;
    public bool IsPause => gameState == GameState.Pause;
    public bool IsMenu => gameState == GameState.Menu;
}