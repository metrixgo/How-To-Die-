using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwimPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerCam;

    private float Speed = 1000f;
    private float WaterY = -0.1f;

    private GameInput input;
    private InputAction moveAction;

    private Rigidbody rb;
    private CinemachinePanTilt camPanTilt;

    private Vector3 move;

    private void Awake()
    {
        input = new GameInput();
        moveAction = input.Player.Move;

        rb = GetComponent<Rigidbody>();
        camPanTilt = GetComponent<CinemachinePanTilt>();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        rb.AddForce(Vector3.down * 10f);
        camPanTilt.TiltAxis.Value = playerCam.eulerAngles.x;
        camPanTilt.PanAxis.Value = playerCam.eulerAngles.y;
    }

    private void Update()
    {
        float forward = moveAction.ReadValue<Vector2>().y;
        move = transform.forward * forward * Speed * Time.deltaTime;
        if (transform.position.y > WaterY) move.y = 0f;
        rb.AddForce(move);
        if(transform.position.y > WaterY) rb.AddForce(Vector3.down * 5f);
        else rb.AddForce(Vector3.up * 1f);
    }
}
