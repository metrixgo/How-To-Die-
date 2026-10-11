using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Audios")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource rainAmbience;
    [SerializeField] private AudioSource windAmbience;
    [SerializeField] private AudioSource writingEffectsPlayer;

    [Header("Rendering")]
    [SerializeField] private Image gameScreen;
    [SerializeField] private Image backScreen;
    [SerializeField] private Image grainScreen;
    [SerializeField] private Image frontScreen;
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private TextMeshProUGUI quitPrompt;

    public static GameManager Instance { get; private set; }
    public Color grainScreenColor { get; private set; } = new Color(1f, 1f, 1f, 2f / 255f);
    public float cutSceneVolume { get; private set; } = 0.3f;
    public float sceneVolume { get; private set; } = 0.1f;
    public bool inCutScene { get; private set; } = true;

    private GameInput input;
    private InputAction pauseAction;
    private InputAction skipAction;

    private bool started = false;
    private float quitProgress = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        input = new GameInput();
        pauseAction = input.Game.Pause;
        skipAction = input.Game.Skip;
    }

    private void OnEnable()
    {
        input?.Enable();
    }

    private void OnDisable()
    {
        input?.Disable();
    }

    private void Update()
    {
        if (!started) return;
        if (pauseAction.IsPressed()) quitProgress += Time.deltaTime / 5f;
        else quitProgress -= Time.deltaTime;
        if (quitProgress > 1) Application.Quit();
        else if (quitProgress < 0) quitProgress = 0;
        quitPrompt.color = Color.Lerp(Color.clear, Color.white, quitProgress);
        frontScreen.color = Color.Lerp(Color.clear, Color.black, quitProgress);
    }

    public void StartGame() => started = true;
    public void EnableRaycast(bool b) => frontScreen.raycastTarget = !b;
    public void SetGameScreen(Color c) => gameScreen.color = c;
    public void SetMusic(float f) => audioMixer.SetFloat("MusicVolume", ToDb(f));
    public void SetEffect(float f) => audioMixer.SetFloat("EffectsVolume", ToDb(f));
    private float ToDb(float f) => f <= 0.0001f ? -80f : 20f * Mathf.Log10(f);

    public IEnumerator EnterCutScene(float l)
    {
        inCutScene = true;

        float t = 0;
        while (t < l)
        {
            audioMixer.SetFloat("MusicVolume", ToDb(Mathf.Lerp(sceneVolume, cutSceneVolume, t / l)));
            backScreen.color = Color.Lerp(Color.clear, Color.black, t / l);
            grainScreen.color = Color.Lerp(Color.clear, grainScreenColor, t / l);

            t += Time.deltaTime;
            yield return null;
        }
        audioMixer.SetFloat("MusicVolume", ToDb(cutSceneVolume));
        backScreen.color = Color.black;
        grainScreen.color = grainScreenColor;
    }

    public IEnumerator ExitCutScene(float l)
    {
        float t = 0;
        while (t < l)
        {
            audioMixer.SetFloat("MusicVolume", ToDb(Mathf.Lerp(cutSceneVolume, sceneVolume, t / l)));
            backScreen.color = Color.Lerp(Color.black, Color.clear, t / l);
            grainScreen.color = Color.Lerp(grainScreenColor, Color.clear, t / l);

            t += Time.deltaTime;
            yield return null;
        }
        audioMixer.SetFloat("MusicVolume", ToDb(sceneVolume));
        backScreen.color = Color.clear;
        grainScreen.color = Color.clear;

        inCutScene = false;
    }

    public IEnumerator DisplayDialogue(string content)
    {
        writingEffectsPlayer.Play();
        dialogue.text = "";
        dialogue.color = Color.white;

        float t = 0, gap = 0.05f;
        int idx = 0;
        while (idx < content.Length)
        {
            t += Time.deltaTime;

            if (t >= gap)
            {
                t -= gap;
                dialogue.text += content[idx++];
            }

            if (skipAction.WasPressedThisFrame()) break;

            yield return null;
        }
        dialogue.text = content;

        writingEffectsPlayer.Stop();

        yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => skipAction.WasPressedThisFrame());

        t = 0;
        float l = 0.2f;
        while (t < l)
        {
            dialogue.color = Color.Lerp(Color.white, Color.clear, t / l);
            t += Time.deltaTime;
            yield return null;
        }
    }
}