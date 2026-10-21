using UnityEngine;
using UnityEngine.InputSystem;

public class DriveCar : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioSource idle;
    [SerializeField] private AudioSource acc;

    private GameInput input;
    private InputAction driveAction;
    private Vector2 drive;

    private float curAngle = 0f;
    private float curVelocity = 0f;

    private void Awake()
    {
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
        drive = driveAction.ReadValue<Vector2>();

        curAngle = Mathf.Clamp(curAngle + drive.x * Time.deltaTime * 80f, -40f, 40f);
        if (Mathf.Abs(drive.x) < 0.01f) curAngle = 0f;
        curVelocity = Mathf.Clamp(curVelocity + (drive.y - 0.1f) * Time.deltaTime / 70f, 0f, 0.1f);

        transform.Rotate(transform.up * curAngle * Time.deltaTime);
        transform.position += transform.forward * curVelocity;

        acc.volume = curVelocity / 0.1f;
    }
}