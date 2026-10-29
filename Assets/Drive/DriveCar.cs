using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class DriveCar : MonoBehaviour
{
    public float acce = 0.02f;
    public float frict = 0.006f;
    public float maxs = 0.16f;

    [Header("Sounds")]
    [SerializeField] private AudioSource idle;
    [SerializeField] private AudioSource acc;
    [SerializeField] private AudioSource breaks;
    [SerializeField] private AudioSource writingEffectsPlayer;

    [Header("Car")]
    [SerializeField] private Transform handle;
    [SerializeField] private Transform fuel;
    [SerializeField] private Transform spin;
    [SerializeField] private Transform kph;
    [SerializeField] private Transform heat;
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject fuelLight;

    [Header("Dialogue")]
    [SerializeField] private GameObject dScreen;
    [SerializeField] private TextMeshProUGUI dContent;
    [SerializeField] private GameObject focus;

    private float fuelAng = 30f;
    private float spinAng = -100f;
    private float kphAng = -160f;
    private float heatAng = -40f;

    private SplineAnimate sa;

    private GameInput input;
    private InputAction driveAction;
    private InputAction skipAction;
    private Vector2 drive;

    private float curSpeed = 0f;
    private float curPos = 0f;

    private float prevAng;

    public bool stopped = false;
    public bool pickedUpPhone = false;

    private void Awake()
    {
        sa = GetComponent<SplineAnimate>();

        input = new GameInput();
        driveAction = input.Player.Move;
        driveAction = input.Game.Skip;

        prevAng = transform.eulerAngles.y;
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
        if (GameManager.Instance.InCutScene || stopped) return;

        drive = driveAction.ReadValue<Vector2>();
        if (fuel.localEulerAngles.y > fuelAng)
        {
            drive.y = Mathf.Min(drive.y, 0f);
            idle.volume = 0.2f;
            if (Mathf.Abs(curSpeed) < 0.001f && !stopped)
            {
                stopped = true;
                focus.SetActive(true);
                StartCoroutine(EnterDeath());
            }
        }

        float prevSpeed = curSpeed;
        if (drive.y > 0.01f) curSpeed += drive.y * acce * Time.deltaTime;
        else curSpeed -= drive.y * acce * 2.5f * Time.deltaTime;
        curSpeed -= frict * Time.deltaTime;
        curSpeed = Mathf.Clamp(curSpeed, 0f, maxs);
        acc.volume = drive.y > 0.01f ? curSpeed / maxs : 0f;
        breaks.volume = drive.y < -0.01f && curSpeed > 0.01f ? 0.05f : 0f;

        float deltaSpeed = (curSpeed - prevSpeed) / Time.deltaTime / acce;
        float curZ = cam.localPosition.z;
        if (deltaSpeed > 0.01f) curZ = Mathf.Lerp(curZ, 0.04f, Time.deltaTime * 3f);
        else if (deltaSpeed < -0.01f && drive.y < -0.01f) curZ = Mathf.Lerp(curZ, 0.06f, Time.deltaTime * 3f);
        else curZ = Mathf.Lerp(curZ, 0.05f, Time.deltaTime * 3f);

        cam.localPosition = new Vector3(cam.localPosition.x, cam.localPosition.y, curZ);

        curPos += curSpeed * Time.deltaTime;

        curPos %= 1f;

        sa.NormalizedTime = curPos;

        float deltaAng = Mathf.DeltaAngle(prevAng, transform.eulerAngles.y);
        prevAng = transform.eulerAngles.y;
        Quaternion goalAng = Quaternion.Euler(handle.localEulerAngles.x, handle.localEulerAngles.y, -180f - deltaAng / Time.deltaTime);
        handle.localRotation = Quaternion.Slerp(handle.localRotation, goalAng, Time.deltaTime);

        float deltaFuel = 0.1f + Mathf.Max(drive.y, 0f) * 10.2f;
        fuel.Rotate(Vector3.up * deltaFuel * Time.deltaTime);
        if (fuel.localEulerAngles.y > fuelAng * 0.8f && !fuelLight.activeSelf) fuelLight.SetActive(true);
        spin.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * spinAng, curSpeed / maxs);
        kph.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * kphAng, curSpeed / maxs);
        heat.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * heatAng, curSpeed / maxs);
    }

    private IEnumerator EnterDeath()
    {
        dScreen.SetActive(true);
        yield return StartCoroutine(DisplayDialogue("What?"));
        yield return StartCoroutine(DisplayDialogue("My car is dead..."));
        yield return StartCoroutine(DisplayDialogue("I need to go get my phone. I think I left it in my back trunk."));
        dScreen.SetActive(false);
        yield return new WaitUntil(() => pickedUpPhone);
    }

    private IEnumerator DisplayDialogue(string content)
    {
        writingEffectsPlayer.Play();

        float t = 0f, gap = 0.04f;
        int idx = 0;
        while (idx < content.Length)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                dContent.text += content[idx++];
            }
            if (skipAction.WasPressedThisFrame())
            {
                dContent.text = content;
                break;
            }
            yield return null;
        }

        writingEffectsPlayer.Stop();
        dContent.text = content;

        yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => skipAction.WasPressedThisFrame());
    }
}