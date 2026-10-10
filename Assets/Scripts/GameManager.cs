using System;
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
    [SerializeField] private Image frontScreen;
    [SerializeField] private GameObject ask;
    [SerializeField] private GameObject sleep;
    [SerializeField] private TextMeshProUGUI dialogue;
    [SerializeField] private TextMeshProUGUI quitPrompt;

    private Vignette eye;

    private GameInput input;
    private InputAction pauseAction;
    private InputAction skipAction;

    private float quitProgress = 0f;

    private void Awake()
    {
        volume.profile.TryGet<Vignette>(out eye);

        input = new GameInput();
        pauseAction = input.Game.Pause;
        skipAction = input.Game.Skip;
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
        if (pauseAction.IsPressed()) quitProgress += Time.deltaTime / 6f;
        quitPrompt.color = Color.Lerp(Color.clear, Color.white, quitProgress);
        if (quitProgress > 1) Application.Quit();
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
        ask.SetActive(false);
        sleep.SetActive(false);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator QuitGame()
    {
        ask.SetActive(false);
        sleep.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        float t = 0f, l = 3f, d = 0.5f;
        while(t < l)
        {
            rainAmbience.volume = Mathf.Lerp(1f, 1f - d, t / l);
            windAmbience.volume = Mathf.Lerp(1f, 1f - d, t / l);
            eye.intensity.value = Mathf.Lerp(0f, d, t / l);
            frontScreen.color = Color.Lerp(Color.clear, Color.black * d, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        l = 3f;
        while (t < l)
        {
            rainAmbience.volume = Mathf.Lerp(1f - d, 1f, t / l);
            windAmbience.volume = Mathf.Lerp(1f - d, 1f, t / l);
            eye.intensity.value = Mathf.Lerp(d, 0f, t / l);
            frontScreen.color = Color.Lerp(Color.black * d, Color.clear, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);
        t = 0f;
        l = 8f;
        while (t < l)
        {
            rainAmbience.volume = Mathf.Lerp(1f, 0f, t / l);
            windAmbience.volume = Mathf.Lerp(1f, 0f, t / l);
            eye.intensity.value = Mathf.Lerp(0f, 1f, t / l);
            frontScreen.color = Color.Lerp(Color.clear, Color.black, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        Application.Quit();
    }

    private IEnumerator DisplayDialogue(string content)
    {
        writingEffectsPlayer.Play();
        dialogue.text = "";
        dialogue.color = Color.white;

        float t = 0, gap = 0.02f;
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
        float l = 0.5f;
        while (t < l)
        {
            dialogue.color = Color.Lerp(Color.white, Color.clear, t/ l);
            t += Time.deltaTime;
            yield return null;
        }
    }
}