using System.Collections;
using UnityEngine;

public class CarDoor : MonoBehaviour
{
    [SerializeField] private AudioClip open;
    [SerializeField] private AudioClip close;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject driver;

    private bool opened = false;
    private bool opening = false;
    private AudioSource ad;
    private Interactable ib;

    Quaternion startR = Quaternion.Euler(270f, 0f, 0f);
    Quaternion endR = Quaternion.Euler(270f, 40f, 0f);

    private void Awake()
    {
        ad = GetComponent<AudioSource>();
        ib = GetComponent<Interactable>();
    }

    public void InteractDoor()
    {
        if (opening) return;

        opened = !opened;
        opening = true;
        ib.enabled = false;

        if (opened) StartCoroutine(OpenDoor());
        else StartCoroutine(CloseDoor());
    }

    private IEnumerator OpenDoor()
    {
        float t = 0;
        float length = 0.5f;
        bool flg = false;
        ad.PlayOneShot(open);
        while (t < length)
        {
            transform.localRotation = Quaternion.Lerp(startR, endR, t / length);
            t += Time.deltaTime;
            if(t > 0.2f && !flg)
            {
                flg = true;
                player.transform.localPosition = new Vector3(-1.22281504f, 0.0249082576f, -0.460198969f);
                player.gameObject.SetActive(true);
                player.CanDo.Move = false;
                driver.SetActive(false);
            }

            yield return null;
        }

        transform.localRotation = endR;

        yield return new WaitForSeconds(0.8f);
        player.CanDo.Move = true;
        opening = false;
        ib.enabled = true;
    }

    private IEnumerator CloseDoor()
    {
        player.gameObject.SetActive(false);
        driver.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        float t = 0;
        float length = 0.5f;
        bool played = false;
        while (t < length)
        {
            transform.localRotation = Quaternion.Lerp(endR, startR, t / length);
            t += Time.deltaTime;
            if (t > 0.3f && !played)
            {
                played = true;
                ad.PlayOneShot(close);
            }
            yield return null;
        }

        transform.localRotation = startR;

        opening = false;
        ib.enabled = true;
    }
}