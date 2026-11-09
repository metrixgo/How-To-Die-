using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CarTrunk : MonoBehaviour
{
    [SerializeField] private AudioClip open;
    [SerializeField] private AudioClip close;
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject trunkCam;
    [SerializeField] private GameObject bookCam;
    [SerializeField] private GameObject key;
    [SerializeField] private Interactable carDoor;
    [SerializeField] private AudioSource globalAd;
    [SerializeField] private GameObject phone;
    [SerializeField] private AudioClip pickUp;
    [SerializeField] private AudioClip pickUpKey;
    [SerializeField] private GameObject sideWalls;
    [SerializeField] private Transform backCar;
    [SerializeField] private AudioSource horn;
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GameObject focus;
    [SerializeField] private AudioClip skidding;
    [SerializeField] private AudioClip crash;

    [Header("AfterCrash")]
    [SerializeField] private GameObject crashedStuff;
    [SerializeField] private GameObject cam1;
    [SerializeField] private GameObject cam2;
    [SerializeField] private GameObject cam3;

    private bool opened = false;
    private bool opening = false;
    private AudioSource ad;
    private Interactable ib;

    private bool pickedUpPhone = false;
    private bool focusOnBook = false;
    private bool startCrashing = false;

    Quaternion startR = Quaternion.Euler(-90f, 0f, 0f);
    Quaternion endR = Quaternion.Euler(-20f, 0f, 0f);

    private GameInput input;
    private InputAction returnAction;

    private CinemachineBlendDefinition blend0;
    private CinemachineBlendDefinition blend1;
    private CinemachineBlendDefinition blend2;

    private void Awake()
    {
        ad = GetComponent<AudioSource>();
        ib = GetComponent<Interactable>();

        input = new GameInput();
        returnAction = input.Game.Return;

        blend0 = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.EaseInOut,
            0f
        );
        blend1 = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.EaseInOut,
            1f
        );
        blend2 = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.EaseInOut,
            2f
        );
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
        if(returnAction.WasPressedThisFrame() && opened && !opening && !startCrashing)
        {
            if (focusOnBook) InteractBook();
            else InteractDoor();
        }
    }

    public void InteractDoor()
    {
        if (opening) return;

        opened = !opened;
        focus.SetActive(false);
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
                brain.DefaultBlend = blend1;
                player.gameObject.SetActive(false);
                trunkCam.SetActive(true);
            }

            yield return null;
        }

        transform.localRotation = endR;

        yield return new WaitForSeconds(0.9f);
        opening = false;
        focus.SetActive(true);
    }

    private IEnumerator CloseDoor()
    {
        if (pickedUpPhone)
        {
            sideWalls.SetActive(true);
            backCar.gameObject.SetActive(true);
            StartCoroutine(Crash());
        }

        brain.DefaultBlend = blend1;
        player.transform.localPosition = new Vector3(-0.00787682831f, 0.025003599f, -2.61117959f);
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
        focus.SetActive(true);
        
        if(!startCrashing) ib.enabled = true;
        player.CanDo.Move = true;
        carDoor.enabled = true;
    }

    private IEnumerator Crash()
    {
        startCrashing = true;
        horn.Play();
        Vector3 startPos = backCar.localPosition;
        Vector3 endPos = new Vector3(0f, 0f, -3f);
        bool skiddled = false, crashed = false, cutScene = false;
        float t = 0f, l = 3f;
        while(t < l)
        {
            if(t > l - 2f && !skiddled)
            {
                skiddled = true;
                horn.PlayOneShot(skidding);
            }
            if (t > l - 0.3f && !crashed)
            {
                crashed = true;
                globalAd.PlayOneShot(crash);
            }
            if (t > l - 0.1f && !cutScene)
            {
                cutScene = true;
                StartCoroutine(GameManager.Instance.EnterCutScene(0.1f));
            }

            horn.volume = Mathf.Lerp(0f, 1f, t / l - 0.5f);
            backCar.localPosition = Vector3.Lerp(startPos, endPos, t / l);
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        horn.Stop();
        crashedStuff.SetActive(true);
        player.gameObject.SetActive(false);
        brain.DefaultBlend = blend0;
        cam1.SetActive(true);
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(GameManager.Instance.ExitCutScene(0.4f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.EnterCutScene(0.1f));
        cam1.SetActive(false);
        cam2.SetActive(true);
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(GameManager.Instance.ExitCutScene(0.4f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.EnterCutScene(0.1f));
        cam2.SetActive(false);
        cam3.SetActive(true);
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(GameManager.Instance.ExitCutScene(0.4f));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.EnterCutScene(0.1f));
        SceneManager.LoadScene("SwimScene");
    }

    public void InteractBook()
    {
        if (opening || startCrashing) return;
        opening = true;
        focus.SetActive(false);
        brain.DefaultBlend = blend2;
        focusOnBook = !focusOnBook;
        bookCam.SetActive(focusOnBook); 
        trunkCam.SetActive(!focusOnBook);
        StartCoroutine(Book());
    }

    private IEnumerator Book()
    {
        yield return new WaitForSeconds(2f);
        opening = false;
        focus.SetActive(true);
    }

    public void PickUpPhone()
    {
        pickedUpPhone = true;
        globalAd.PlayOneShot(pickUp);
        Destroy(phone);
    }

    public void PickUpKey()
    {
        PlayerPrefs.SetInt("Key", 1);
        globalAd.PlayOneShot(pickUpKey);
        Destroy(key);
    }
}
