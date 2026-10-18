using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScene : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private GameObject startingCam;
    [SerializeField] private FixedCamera couchCam;

    [Header("Rendering")]
    [SerializeField] private Volume volume;
    [SerializeField] private Image screen;
    private DepthOfField dof;
    private Vignette v;

    [Header("Sounds")]
    [SerializeField] private AudioReverbZone arz;
    [SerializeField] private AudioLowPassFilter alpf;
    [SerializeField] private AudioSource tinnitusAd;
    [SerializeField] private AudioSource heartbeatAd;
    [SerializeField] private AudioSource clockAd;
    [SerializeField] private AudioSource waterAd;
    [SerializeField] private AudioSource fireAd;
    [SerializeField] private AudioSource smokeAd;
    [SerializeField] private AudioClip tinnitus;
    [SerializeField] private AudioClip heartbeat;

    private Coroutine eyeCR;

    private void Awake()
    {
        volume.profile.TryGet(out dof);
        volume.profile.TryGet(out v);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone dies, because they used up all their time."));
        StartCoroutine(GameManager.Instance.ExitCutScene(3f));
        startingCam.SetActive(true);

        yield return new WaitForSeconds(7.5f);

        tinnitusAd.clip = tinnitus;
        tinnitusAd.volume = 0.2f;
        tinnitusAd.Play();
        heartbeatAd.clip = heartbeat;
        heartbeatAd.volume = 0.8f;
        heartbeatAd.Play();
        waterAd.volume = fireAd.volume = smokeAd.volume = 0.5f;

        float t = 0f, l = 0.5f;
        while (t < l)
        {
            dof.gaussianStart.value = dof.gaussianEnd.value = Mathf.Lerp(10f, 0f, t / l);
            v.intensity.value = Mathf.Lerp(0.3f, 0.4f, t / l);
            screen.color = Color.red * Mathf.Lerp(0f, 0.2f, t / l);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f; l = 4.5f;
        while (t < l)
        {
            dof.gaussianStart.value = Mathf.Lerp(0f, 1f, t / l);
            dof.gaussianEnd.value = Mathf.Lerp(0f, 5f, t / l);
            v.intensity.value = Mathf.Lerp(0.4f, 0.3f, t / l);
            t += Time.deltaTime;
            yield return null;
        }

        eyeCR = StartCoroutine(Eye());

        t = 0f; l = 4f;
        while (t < l)
        {
            tinnitusAd.volume = Mathf.Lerp(0.2f, 0.01f, t / l);
            heartbeatAd.volume = Mathf.Lerp(0.8f, 0.4f, t / l);

            t += Time.deltaTime;
            yield return null;
        }
        tinnitusAd.volume = 0.01f;
        heartbeatAd.volume = 0.4f;

        yield return new WaitForSeconds(4f);
        startingCam.SetActive(false);
        couchCam.gameObject.SetActive(true);

        yield return new WaitForSeconds(10f);

        StopCoroutine(eyeCR);

        t = 0f; l = 10f;
        float iV = v.intensity.value, iD = dof.gaussianStart.value, iDec = arz.decayTime, iR = arz.reverb, iF = alpf.cutoffFrequency;
        Color iC = screen.color;
        while (t < l)
        {
            tinnitusAd.volume = Mathf.Lerp(0.01f, 0f, t / l);
            heartbeatAd.volume = Mathf.Lerp(0.4f, 0.05f, t / l);
            clockAd.spatialBlend = Mathf.Lerp(1f, 0f, t / l);
            clockAd.volume = Mathf.Lerp(0.5f, 1f, t / l);
            waterAd.volume = fireAd.volume = smokeAd.volume = Mathf.Lerp(0.5f, 0f, t / l);
            couchCam.sensitivity = Mathf.Lerp(1f, 0.1f, t / l);
            v.intensity.value = Mathf.Lerp(iV, 1f, t / l);
            dof.gaussianStart.value = Mathf.Lerp(iD, 0f, t / l);
            dof.gaussianEnd.value = Mathf.Lerp(iD, 0f, t / l) * 5f;
            screen.color = Color.Lerp(iC, Color.black, t / l);
            arz.decayTime = Mathf.Lerp(iDec, 50f, t / l);
            arz.reverb = (int)Mathf.Lerp(iR, -500f, t / l);
            alpf.cutoffFrequency = Mathf.Lerp(iF, 1800f, t / l);

            t += Time.deltaTime;
            yield return null;
        }
        screen.color = Color.black;
        clockAd.spatialBlend = 0f;

        yield return new WaitForSeconds(6f);
        yield return StartCoroutine(GameManager.Instance.EnterCutScene(2f));
        SceneManager.LoadScene("DriveScene");
    }

    private IEnumerator Eye()
    {
        float t = 0f;
        while (true)
        {
            dof.gaussianStart.value = Mathf.Sin(t * 2f * Mathf.PI / 4f) + 1f;
            dof.gaussianEnd.value = Mathf.Sin(t * 2f * Mathf.PI / 4f) * 2f + 5f;
            v.intensity.value = Mathf.Sin(t * 2f * Mathf.PI / 8f) / 10f + 0.3f;
            screen.color = ((Mathf.Sin(t * 2f * Mathf.PI / 4f) + 1f) / 2f / 10f + 0.2f) * Color.red;

            t += Time.deltaTime;
            yield return null;
        }
    }
}