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

        curAngle = transform.eulerAngles.y;
    }

    private void Update()
    {
        drive = driveAction.ReadValue<Vector2>();

        curAngle = Mathf.Clamp(curAngle + drive.x * Time.deltaTime, -20f, 20f);
        curVelocity = Mathf.Clamp(curVelocity + (drive.y - 0.5f) * Time.deltaTime, 0f, 40f);

        transform.rotation = Quaternion.Euler(0f, curAngle, 0f);
        transform.position += transform.forward * curVelocity;
    }
}
