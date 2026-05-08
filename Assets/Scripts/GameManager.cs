using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool gameOver;

    private void Awake() => Instance = this;

    public void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("Game Over");
        Time.timeScale = 0f;
    }
}