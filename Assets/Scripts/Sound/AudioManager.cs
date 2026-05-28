using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    None,
    SFX,
    Music,
    Ambient,
}

[System.Serializable]
public class AudioData
{
    public string id;
    public AudioClip clip;
    public AudioType type;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;
    public bool useRandomPitch;
    public float pitchOffset = 0.2f;
    [Tooltip("-1 = infinite")] public int repeatCount = -1;
    public bool playOnAwake = false;
    public bool canFadeIn;
    public bool canFadeOut;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Database")]
    public AudioData[] audioDatabase;

    [Header("Prefabs")]
    public AudioSource musicPrefab;
    public AudioSource sfxPrefab;
    public AudioSource ambientPrefab;

    [Header("Settings")]
    public float soundtrackFadeSpeed = 1.5f;

    private readonly Dictionary<string, AudioData> _audioMap = new();
    private readonly List<AudioSource> _activeSources = new();

    private void Awake() => Instance = this;

    private void Start()
    {
        Initialize();
        foreach (AudioData data in audioDatabase) if (data.playOnAwake) Play(data.id);
    }

    #region Initialize

    private void Initialize()
    {
        foreach (AudioData data in audioDatabase) if (!_audioMap.ContainsKey(data.id)) _audioMap.Add(data.id, data);
    }

    #endregion

    #region Play / Stop

    public void Play(string id)
    {
        if (!_audioMap.TryGetValue(id, out AudioData data))
        {
            Debug.LogWarning($"Audio ID not found: {id}");
            return;
        }
        if (IsSingleChannel(data.type)) FadeOutByType(data.type);
        AudioSource source;
        switch (data.type)
        {
            case AudioType.Music: source = Instantiate(musicPrefab, transform);
                break;
            case AudioType.SFX: source = Instantiate(sfxPrefab, transform);
                break;
            case AudioType.Ambient: source = Instantiate(ambientPrefab, transform);
                break;
            default: source = Instantiate(sfxPrefab, transform);
                break;
        }
        source.name = $"AudioSource_{id}";
        source.clip = data.clip;
        float startVolume = data.canFadeIn ? 0f : data.volume;
        float targetVolume = data.volume;
        source.volume = startVolume;
        if (data.useRandomPitch) source.pitch = Random.Range(data.pitch - data.pitchOffset, data.pitch + data.pitchOffset);
        else source.pitch = data.pitch;
        source.loop = data.repeatCount == -1;
        source.Play();
        _activeSources.Add(source);
        if (data.canFadeIn) StartCoroutine(Fade(source, 0f, targetVolume, soundtrackFadeSpeed));
        StartCoroutine(HandleLifetime(source, data));
    }
    public void Stop(string id)
    {
        List<AudioSource> sourcesToStop = new();
        foreach (AudioSource source in _activeSources)
        {
            if (source == null) continue;
            AudioData data = GetAudioData(source.clip);
            if (data == null) continue;
            if (data.id == id) sourcesToStop.Add(source);
        }
        foreach (AudioSource source in sourcesToStop)
        {
            AudioData data = GetAudioData(source.clip);
            if (data != null && data.canFadeOut) StartCoroutine(FadeOutAndDestroy(source));
            else
            {
                _activeSources.Remove(source);
                Destroy(source.gameObject);
            }
        }
    }

    public void PlayAnySoundtrack()
    {
        StopAllSoundtracks();
        List<AudioData> soundtrackList = new();
        foreach (AudioData data in audioDatabase)
        {
            if (data.type == AudioType.Music) soundtrackList.Add(data);
        }
        if (soundtrackList.Count > 0)
        {
            int randomIndex = Random.Range(0, soundtrackList.Count);
            Play(soundtrackList[randomIndex].id);
        }
    }
    public void StopAllSoundtracks()
    {
        foreach (AudioData data in audioDatabase) if (data.type == AudioType.Music) Stop(data.id);
    }

    #endregion

    #region Lifetime

    private IEnumerator HandleLifetime(AudioSource source, AudioData data)
    {
        if (data.repeatCount == -1) yield break;
        int played = 0;
        while (played <= data.repeatCount)
        {
            if (source == null) yield break;
            yield return new WaitForSeconds(data.clip.length);
            if (source == null) yield break;
            played++;
            if (played <= data.repeatCount && source != null) source.Play();
        }
        if (source != null)
        {
            _activeSources.Remove(source);
            Destroy(source.gameObject);
        }
    }

    #endregion

    #region Soundtrack

    private void FadeOutByType(AudioType type)
    {
        List<AudioSource> sourcesToFade = new();
        foreach (AudioSource source in _activeSources)
        {
            if (source == null) continue;
            AudioData data = GetAudioData(source.clip);
            if (data == null) continue;
            if (data.type == type) sourcesToFade.Add(source);
        }
        foreach (AudioSource source in sourcesToFade)
        {
            AudioData data = GetAudioData(source.clip);
            if (data != null && data.canFadeOut) StartCoroutine(FadeOutAndDestroy(source));
            else
            {
                _activeSources.Remove(source);
                Destroy(source.gameObject);
            }
        }
    }

    private IEnumerator Fade(AudioSource source, float from, float to, float speed)
    {
        if (source == null) yield break;
        source.volume = from;
        while (source != null && Mathf.Abs(source.volume - to) > 0.01f)
        {
            if (source == null) yield break;
            source.volume = Mathf.MoveTowards(source.volume, to, speed * Time.deltaTime);
            yield return null;
        }
        if (source != null) source.volume = to;
    }

    private IEnumerator FadeOutAndDestroy(AudioSource source)
    {
        if (source == null) yield break;
        while (source != null && source.volume > 0.01f)
        {
            source.volume = Mathf.MoveTowards(source.volume, 0f, soundtrackFadeSpeed * Time.deltaTime);
            yield return null;
        }

        if (source != null)
        {
            _activeSources.Remove(source);
            Destroy(source.gameObject);
        }
    }

    #endregion

    #region Helpers

    private AudioData GetAudioData(AudioClip clip)
    {
        foreach (AudioData data in audioDatabase) if (data.clip == clip) return data;
        return null;
    }

    private bool IsSingleChannel(AudioType type)
    {
        return type == AudioType.Music || type == AudioType.Ambient;
    }

    #endregion
}