using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.Windows.WebCam;

public class DriveCar : MonoBehaviour
{
    public float acce = 5f;
    public float frict = 0.2f;
    public float maxs = 20f;

    [Header("Sounds")]
    [SerializeField] private AudioSource idle;
    [SerializeField] private AudioSource acc;
    [SerializeField] private AudioSource breaks;

    [Header("Car")]
    [SerializeField] private Transform handle;
    [SerializeField] private Transform fuel;
    [SerializeField] private Transform spin;
    [SerializeField] private Transform kph;
    [SerializeField] private Transform heat;
    [SerializeField] private Transform cam;

    private float fuelAng = 30f;
    private float spinAng = -100f;
    private float kphAng = -160f;
    private float heatAng = -40f;

    private SplineAnimate sa;

    private GameInput input;
    private InputAction driveAction;
    private Vector2 drive;

    private float curSpeed = 0f;
    private float curPos = 0f;

    private float prevAng;

    private void Awake()
    {
        sa = GetComponent<SplineAnimate>();

        input = new GameInput();
        driveAction = input.Player.Move;

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
        if (GameManager.Instance.InCutScene) return;

        drive = driveAction.ReadValue<Vector2>();
        float prevSpeed = curSpeed;
        curSpeed += fuel.localEulerAngles.y > fuelAng && drive.y > 0.01f ? 0f : drive.y * acce * Time.deltaTime;
        curSpeed -= frict * Time.deltaTime;
        curSpeed = Mathf.Clamp(curSpeed, 0f, maxs);
        acc.volume = drive.y > 0.01f ? curSpeed / maxs : 0f;
        breaks.volume = drive.y < -0.01f && curSpeed > 0.01f ? 1f : 0f;

        float deltaSpeed = curSpeed - prevSpeed;
        float curZ = cam.localPosition.z;
        if (deltaSpeed > 0.01f) curZ = Mathf.Lerp(curZ, 0f, Time.deltaTime * 3f);
        else if (deltaSpeed < -0.01f) curZ = Mathf.Lerp(curZ, 0.1f, Time.deltaTime * 3f);
        else curZ = Mathf.Lerp(curZ, 0.05f, Time.deltaTime * 3f);

        cam.localPosition = new Vector3(cam.localPosition.x, cam.localPosition.y, curZ);

        curPos += curSpeed * Time.deltaTime;
        curPos %= 1f;

        sa.NormalizedTime = curPos;

        float deltaAng = transform.eulerAngles.y - prevAng;
        prevAng = transform.eulerAngles.y;
        Vector3 goalAng = new Vector3(handle.localEulerAngles.x, handle.localEulerAngles.y, -180f - deltaAng / Time.deltaTime);
        handle.localEulerAngles = Vector3.Lerp(handle.localEulerAngles, goalAng, Time.deltaTime * 5f);

        float deltaFuel = curSpeed + 0.1f;
        fuel.Rotate(Vector3.up * deltaFuel * Time.deltaTime);
        spin.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * spinAng, curSpeed / maxs);
        kph.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * kphAng, curSpeed / maxs);
        heat.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.up * heatAng, curSpeed / maxs);
    }
}