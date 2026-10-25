using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class DriveCar : MonoBehaviour
{
    public float acce = 5f;
    public float frict = 0.2f;
    public float maxs = 20f;

    [Header("Sounds")]
    [SerializeField] private AudioSource idle;
    [SerializeField] private AudioSource acc;

    private SplineAnimate sa;

    private GameInput input;
    private InputAction driveAction;
    private Vector2 drive;

    private float curSpeed = 0f;
    private float curPos = 0f;

    private void Awake()
    {
        sa = GetComponent<SplineAnimate>();

        input = new GameInput();
        driveAction = input.Player.Move;
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
        curSpeed += drive.y * acce * Time.deltaTime;
        curSpeed -= frict * Time.deltaTime;
        curSpeed = Mathf.Clamp(curSpeed, 0f, maxs);
        acc.volume = Mathf.Abs(drive.y) > 0.01f ? curSpeed / maxs : 0f;

        curPos += curSpeed / 100f;
        curPos %= 1f;

        sa.NormalizedTime = curPos;
    }
}