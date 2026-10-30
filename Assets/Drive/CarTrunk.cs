using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarTrunk : MonoBehaviour
{
    [Header("Normal")]
    [SerializeField] private AudioClip open;
    [SerializeField] private AudioClip close;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject trunkCam;
    [SerializeField] private Interactable carDoor;

    [Header("Items")]
    [SerializeField] private AudioSource globalAd;
    [SerializeField] private GameObject phone;
    [SerializeField] private AudioClip pickUp;

    private bool opened = false;
    private bool opening = false;
    private AudioSource ad;
    private Interactable ib;

    private bool pickedUp = false;

    Quaternion startR = Quaternion.Euler(-90f, 0f, 0f);
    Quaternion endR = Quaternion.Euler(-20f, 0f, 0f);

    private GameInput input;
    private InputAction returnAction;

    private void Awake()
    {
        ad = GetComponent<AudioSource>();
        ib = GetComponent<Interactable>();

        input = new GameInput();
        returnAction = input.Game.Return;
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
        if(returnAction.WasPressedThisFrame() && opened && !opening)
        {
            InteractDoor();
        }
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
        carDoor.enabled = false;

        float t = 0;
        float length = 0.5f;
        bool flg = false;
        ad.PlayOneShot(open);
        while (t < length)
        {
            transform.localRotation = Quaternion.Lerp(startR, endR, t / length);
            t += Time.deltaTime;
            if (t > 0.4f && !flg)
            {
                flg = true;
                player.gameObject.SetActive(false);
                trunkCam.SetActive(true);
            }

            yield return null;
        }

        transform.localRotation = endR;

        yield return new WaitForSeconds(0.9f);
        opening = false;
    }

    private IEnumerator CloseDoor()
    {
        player.gameObject.SetActive(true);
        player.CanDo.Move = false;
        trunkCam.SetActive(false);
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
        player.CanDo.Move = true;
        carDoor.enabled = true;
    }

    public void PickUpPhone()
    {
        pickedUp = true;
        globalAd.PlayOneShot(pickUp);
        Destroy(phone);
    }
}
