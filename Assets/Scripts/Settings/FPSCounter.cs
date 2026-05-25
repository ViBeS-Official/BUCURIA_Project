using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text fpsText;

    [Header("Settings")]
    public float updateInterval = 0.5f;

    private float _timer;
    private int _frames;
    private float _fps;

    private void Update() => CalculateFPS();

    private void CalculateFPS()
    {
        _frames++;
        _timer += Time.unscaledDeltaTime;
        if (_timer >= updateInterval)
        {
            _fps = _frames / _timer;
            _frames = 0;
            _timer = 0;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (!fpsText) return;
        fpsText.text = $"FPS: {Mathf.RoundToInt(_fps)}";
    }
}