using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioSource rainAmbience;
    [SerializeField] private AudioSource windAmbience;
    [SerializeField] private AudioSource writingEffectsPlayer;

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

    private Vignette eye;

    private GameInput input;
    private InputAction pauseAction;
    private InputAction skipAction;

    private Color grainScreenColor = new Color(1f, 1f, 1f, 5f / 255f);
    private Color frontScreenColor = new Color(0f, 0f, 0f, 50f / 255f);

    private bool started = false;

    private float quitProgress = 0f;

    private void Awake()
    {
        volume.profile.TryGet<Vignette>(out eye);

        input = new GameInput();
        pauseAction = input.Game.Pause;
        skipAction = input.Game.Skip;

        grainScreen.color = grainScreenColor;
        frontScreen.color = frontScreenColor;
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
        if (started)
        {
            if (pauseAction.IsPressed()) quitProgress += Time.deltaTime / 6f;
            quitPrompt.color = Color.Lerp(Color.clear, Color.white, quitProgress);
            frontScreen.color = Color.Lerp(Color.clear, Color.black, quitProgress);
            if (quitProgress > 1) Application.Quit();
        }
    }

    public void Ask()
    {
        StartCoroutine(StartGame());
    }

    public void Sleep()
    {
        StartCoroutine(QuitGame());
    }

    private IEnumerator StartGame()
    {
        started = true;
        frontScreen.raycastTarget = true;
        StartCoroutine(HideButtons());

        float t = 0f, l = 3f, d = 0.7f;
        while (t < l)
        {
            rainAmbience.volume = Mathf.Lerp(1f, 1f - d, t / l);
            windAmbience.volume = Mathf.Lerp(1f, 1f - d, t / l);
            frontScreen.color = Color.Lerp(frontScreenColor, Color.clear, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DisplayDialogue("Mommy!"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DisplayDialogue("Yes, honey?"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DisplayDialogue("Mom... how do people die?"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(DisplayDialogue("......"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DisplayDialogue("Mom...?"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DisplayDialogue("Why are you asking this?"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DisplayDialogue("You said people all die one day..."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DisplayDialogue("But I don't understand how people die..."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(DisplayDialogue("Well... this is a deep question..."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DisplayDialogue("I'm not sure how to describe it to you..."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DisplayDialogue("Mom! Please? I want to know..."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(DisplayDialogue("Okay."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(DisplayDialogue("People die in different ways... because of different reasons."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(DisplayDialogue("Someone dies peacefully, without any struggle or resistance."));
    }

    private IEnumerator QuitGame()
    {
        frontScreen.raycastTarget = true;
        StartCoroutine(HideButtons());

        float t = 0f, l = 2f, d = 0.5f;
        while (t < l)
        {
            rainAmbience.volume = Mathf.Lerp(1f, 1f - d, t / l);
            windAmbience.volume = Mathf.Lerp(1f, 1f - d, t / l);
            eye.intensity.value = Mathf.Lerp(0f, d, t / l);
            frontScreen.color = Color.Lerp(frontScreenColor, frontScreenColor * (1f + d), t / l);

            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        l = 2f;
        while (t < l)
        {
            rainAmbience.volume = Mathf.Lerp(1f - d, 1f, t / l);
            windAmbience.volume = Mathf.Lerp(1f - d, 1f, t / l);
            eye.intensity.value = Mathf.Lerp(d, 0f, t / l);
            frontScreen.color = Color.Lerp(frontScreenColor * (1f + d), frontScreenColor, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        t = 0f;
        l = 6f;
        while (t < l)
        {
            rainAmbience.volume = Mathf.Lerp(1f, 0f, t / l);
            windAmbience.volume = Mathf.Lerp(1f, 0f, t / l);
            eye.intensity.value = Mathf.Lerp(0f, 1f, t / l);
            frontScreen.color = Color.Lerp(frontScreenColor, Color.black, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        Application.Quit();
    }

    private IEnumerator HideButtons()
    {
        float t = 0f, l = 1f;
        while (t < l)
        {
            ask.alpha = Mathf.Lerp(1f, 0f, t / l);
            sleep.alpha = Mathf.Lerp(1f, 0f, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        ask.gameObject.SetActive(false);
        sleep.gameObject.SetActive(false);
    }

    private IEnumerator DisplayDialogue(string content)
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