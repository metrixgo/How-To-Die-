using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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
    [SerializeField] private AudioClip tinnitus;
    [SerializeField] private AudioClip heartbeat;
    [SerializeField] private AudioSource tinnitusAd;
    [SerializeField] private AudioSource heartbeatAd;
    [SerializeField] private AudioSource clockAd;

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
        tinnitusAd.volume = 0.4f;
        tinnitusAd.Play();
        heartbeatAd.clip = heartbeat;
        tinnitusAd.volume = 0.8f;
        heartbeatAd.Play();
        dof.gaussianStart.value = 1f;
        dof.gaussianEnd.value = 2f;
        v.intensity.value = 0.3f;

        yield return new WaitForSeconds(7.5f);

        eyeCR = StartCoroutine(Eye());

        float t = 0f, l = 4f;
        while (t < l)
        {
            tinnitusAd.volume = Mathf.Lerp(0.4f, 0.05f, t / l);
            heartbeatAd.volume = Mathf.Lerp(0.8f, 0.4f, t / l);

            t += Time.deltaTime;
            yield return null;
        }
        tinnitusAd.volume = 0.05f;
        heartbeatAd.volume = 0.4f;

        yield return new WaitForSeconds(4f);
        startingCam.SetActive(false);
        couchCam.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        StopCoroutine(eyeCR);

        t = 0f; l = 10f; float iV = v.intensity.value; float iD = dof.gaussianStart.value;
        while (t < l)
        {
            heartbeatAd.volume = Mathf.Lerp(0.4f, 0.8f, t / l);
            clockAd.spatialBlend = Mathf.Lerp(1f, 0f, t / l);
            couchCam.sensitivity = Mathf.Lerp(1f, 0.1f, t / l);
            v.intensity.value = Mathf.Lerp(iV, 1f, t / l);
            dof.gaussianStart.value = Mathf.Lerp(iD, 0f, t / l);
            dof.gaussianEnd.value = Mathf.Lerp(iD, 0f, t / l) * 2f;
            screen.color = Color.Lerp(Color.clear, Color.black, t / l);

            t += Time.deltaTime;
            yield return null;
        }
        screen.color = Color.black;
        clockAd.spatialBlend = 0f;

        yield return new WaitForSeconds(3f);
        StartCoroutine(GameManager.Instance.EnterCutScene(5f));
    }

    private IEnumerator Eye()
    {
        float t = 0f;
        while (true)
        {
            dof.gaussianStart.value = Mathf.Sin(t * 2f * Mathf.PI / 4f);
            dof.gaussianEnd.value = Mathf.Sin(t * 2f * Mathf.PI / 4f) * 2f;
            v.intensity.value = Mathf.Sin(t * 2f * Mathf.PI / 8f) / 10f + 0.3f;

            t += Time.deltaTime;
            yield return null;
        }
    }
}
