using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] private Volume volume;
    [SerializeField] private Image gameScreen;
    [SerializeField] private Image backScreen;
    [SerializeField] private Image grainScreen;
    [SerializeField] private Image frontScreen;
    [SerializeField] private CanvasGroup ask;
    [SerializeField] private CanvasGroup sleep;
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private TextMeshProUGUI quitPrompt;

    public static CanvasManager Instance { get; private set; }

    public bool inCutScene { get; private set; } = true;

    private GameInput input;
    private InputAction pauseAction;
    private InputAction skipAction;

    private float cutSceneVolume = 0.3f;
    private float sceneVolume = 0.1f;
    private Color grainScreenColor = new Color(1f, 1f, 1f, 2f / 255f);

    private Vignette eye;

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

        grainScreen.color = grainScreenColor;

        volume.profile.TryGet<Vignette>(out eye);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        if (pauseAction.IsPressed()) quitProgress += Time.deltaTime / 5f;
        else quitProgress -= Time.deltaTime;
        if (quitProgress > 1) Application.Quit();
        else if (quitProgress < 0) quitProgress = 0;
        quitPrompt.color = Color.Lerp(Color.clear, Color.white, quitProgress);
        frontScreen.color = Color.Lerp(Color.clear, Color.black, quitProgress);
    }

    private IEnumerator EnterCutScene(float l)
    {
        inCutScene = true;

        float t = 0;
        while (t < l)
        {
            SoundManager.Instance.SetMusic(Mathf.Lerp(sceneVolume, cutSceneVolume, t / l));
            backScreen.color = Color.Lerp(Color.clear, Color.black, t / l);
            grainScreen.color = Color.Lerp(Color.clear, grainScreenColor, t / l);

            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ExitCutScene(float l)
    {
        float t = 0;
        while (t < l)
        {
            SoundManager.Instance.SetMusic(Mathf.Lerp(cutSceneVolume, sceneVolume, t / l));
            backScreen.color = Color.Lerp(Color.black, Color.clear, t / l);
            grainScreen.color = Color.Lerp(grainScreenColor, Color.clear, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        inCutScene = false;
    }

    private IEnumerator DisplayDialogue(string content)
    {
        SoundManager.Instance.Write(true);
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

        SoundManager.Instance.Write(false);

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
