using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HospitalScene : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private FixedCamera cam;

    [Header("Rendering")]
    [SerializeField] private Volume volume;
    [SerializeField] private Image screen;
    
    [Header("Audio")]
    [SerializeField] private AudioSource ambience;
    [SerializeField] private AudioSource monitor;
    [SerializeField] private AudioClip beep;
    [SerializeField] private AudioClip line;

    private float heartGap = 1.5f;

    private Vignette eye;
    
    private Coroutine eyeCR;
    private Coroutine monitorCR;

    private void Awake()
    {
        volume.profile.TryGet(out eye);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Mommy!"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Yes, honey?"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Mom... how do people die?"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("......"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Mom...?"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Why are you asking this?"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("You said people all die one day..."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("But I don't understand how people die..."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Well... this is a deep question..."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("I'm not sure how to describe it to you..."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Mom! Please? I want to know..."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Okay."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("People die in different ways... because of different reasons."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone dies peacefully, without any struggle or resistance."));
        StartCoroutine(GameManager.Instance.ExitCutScene(3f));

        eyeCR = StartCoroutine(Eye());
        monitorCR = StartCoroutine(Monitor());

        float t = 0f, l = 20f, iS = cam.sensitivity;
        while(t < l)
        {
            heartGap = Mathf.Lerp(1f, 5f, t / l);
            cam.sensitivity = Mathf.Lerp(iS, 0.1f, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        StopCoroutine(eyeCR);

        t = 0f; l = 14f; float iI = eye.intensity.value; bool flg = false;
        while (t < l)
        {
            eye.intensity.value = Mathf.Lerp(iI, 1f, t / l);
            screen.color = Color.Lerp(Color.clear, Color.black, t / l);
            ambience.volume = Mathf.Lerp(1f, 0.1f, t / l);
            monitor.volume = Mathf.Lerp(1f, 0.4f, t / l);

            if(t > 12f && !flg)
            {
                StopCoroutine(monitorCR);
                monitor.loop = true;
                monitor.clip = line;
                monitor.Play();
                flg = true;
            }

            t += Time.deltaTime;
            yield return null;
        }

        yield return StartCoroutine(GameManager.Instance.EnterCutScene(5f));

        SceneManager.LoadScene("HomeScene");
    }

    private IEnumerator Monitor()
    {
        float t = 0f;
        while (true)
        {
            if (t > heartGap)
            {
                t -= heartGap;
                monitor.PlayOneShot(beep);
            }

            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Eye()
    {
        float t = 0f;
        while (true)
        {
            eye.intensity.value = Mathf.Sin(t * 2f * Mathf.PI / 8f) / 10f + 0.3f;

            t += Time.deltaTime;
            yield return null;
        }
    }
}
