using System.Collections;
using UnityEngine;

public class HomeScene : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private GameObject startingCam;
    [SerializeField] private GameObject couchCam;

    [Header("Sounds")]
    [SerializeField] private AudioClip tinnitus;
    [SerializeField] private AudioClip heartBeat;
    [SerializeField] private AudioSource tinnitusAd;
    [SerializeField] private AudioSource heartBeatAd;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone dies, because they used up all their time."));
        StartCoroutine(GameManager.Instance.ExitCutScene(3f));

        yield return new WaitForSeconds(7.5f);

        tinnitusAd.clip = tinnitus;
        tinnitusAd.Play();
        heartBeatAd.clip = heartBeat;
        heartBeatAd.Play();

        startingCam.SetActive(false);
        couchCam.SetActive(true);
    }
}
