using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [Header("Audios")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource rainAmbience;
    [SerializeField] private AudioSource windAmbience;
    [SerializeField] private AudioSource writingEffectsPlayer;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMusic(float f)
    {
        audioMixer.SetFloat("MusicVolume", ToDb(f));
    }

    public void SetEffect(float f)
    {
        audioMixer.SetFloat("EffectsVolume", ToDb(f));
    }

    private float ToDb(float volume) => volume <= 0.0001f ? -80f : 20f * Mathf.Log10(volume);

    public void Write(bool b)
    {
        if (b) writingEffectsPlayer.Play();
        else writingEffectsPlayer.Stop();
    }
}
