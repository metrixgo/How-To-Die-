using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Audios")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource rainAmbience;
    [SerializeField] private AudioSource windAmbience;
    [SerializeField] private AudioSource writingEffectsPlayer;

    [Header("Canvas")]
    [SerializeField] private Image backScreen;
    [SerializeField] private Image grainScreen;
    [SerializeField] private Image frontScreen;
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private TextMeshProUGUI quitPrompt;

    public static GameManager Instance { get; private set; }
    public Color GrainScreenColor { get; private set; } = new Color(1f, 1f, 1f, 4f / 255f);
    public float CutSceneVolume { get; private set; } = 0.5f;
    public float SceneVolume { get; private set; } = 0.1f;
    public bool InCutScene { get; private set; } = true;

    private GameInput input;
    private InputAction pauseAction;
    private InputAction skipAction;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetSceneVolume(0f);
        SetCutsceneVolume(CutSceneVolume);
        backScreen.color = Color.black;
        grainScreen.color = GrainScreenColor;
    }

    private void Start()
    {
        SetSceneVolume(0f);
        SetCutsceneVolume(CutSceneVolume);
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
        if (SceneManager.GetActiveScene().name == "StartingScene") return;
        if (pauseAction.IsPressed()) quitProgress += Time.deltaTime / 5f;
        else quitProgress -= Time.deltaTime;
        if (quitProgress > 1) Application.Quit();
        else if (quitProgress < 0) quitProgress = 0;
        quitPrompt.color = Color.Lerp(Color.clear, Color.white, quitProgress);
        frontScreen.color = Color.Lerp(Color.clear, Color.black, quitProgress);
    }

    public void EnableRaycast(bool b) => frontScreen.raycastTarget = !b;
    public void SetSceneVolume(float f) => audioMixer.SetFloat("SceneVolume", ToDb(f));
    public void SetCutsceneVolume(float f) => audioMixer.SetFloat("CutsceneVolume", ToDb(f));
    private float ToDb(float f) => f <= 0.0001f ? -80f : 20f * Mathf.Log10(f);

    public IEnumerator EnterCutScene(float l)
    {
        float t = 0;
        while (t < l)
        {
            SetSceneVolume(Mathf.Lerp(1f, 0f, t / l));
            SetCutsceneVolume(Mathf.Lerp(SceneVolume, CutSceneVolume, t / l));
            backScreen.color = Color.Lerp(Color.clear, Color.black, t / l);
            grainScreen.color = Color.Lerp(Color.clear, GrainScreenColor, t / l);

            t += Time.deltaTime;
            yield return null;
        }
        backScreen.color = Color.black;
        grainScreen.color = GrainScreenColor;
        InCutScene = true;
    }

    public IEnumerator ExitCutScene(float l)
    {
        InCutScene = false;
        float t = 0;
        while (t < l)
        {
            SetSceneVolume(Mathf.Lerp(0f, 1f, t / l));
            SetCutsceneVolume(Mathf.Lerp(CutSceneVolume, SceneVolume, t / l));
            backScreen.color = Color.Lerp(Color.black, Color.clear, t / l);
            grainScreen.color = Color.Lerp(GrainScreenColor, Color.clear, t / l);

            t += Time.deltaTime;
            yield return null;
        }
        SetSceneVolume(1f);
        SetCutsceneVolume(SceneVolume);
        backScreen.color = Color.clear;
        grainScreen.color = Color.clear;
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
            if (t >= gap)
            {
                t -= gap;
                dialogue.text += content[idx++];
            }

            if (skipAction.WasPressedThisFrame()) break;

            t += Time.deltaTime;
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
        dialogue.color = Color.clear;
    }
}