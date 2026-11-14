using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SwimPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private AudioSource underwater;
    [SerializeField] private AudioSource seaWave;
    [SerializeField] private Volume volume;
    private Vignette v;

    private float Speed = 200f;
    private float WaterY = 0.15f;

    private Color seaFog = new Color(0.243f, 0.349f, 0.412f);

    private GameInput input;
    private InputAction moveAction;

    private Rigidbody rb;
    private CinemachinePanTilt camPanTilt;
    private AudioLowPassFilter swimFilter;
    private AudioSource swim;

    private Vector3 move;

    private void Awake()
    {
        volume.profile.TryGet(out v);

        input = new GameInput();
        moveAction = input.Player.Move;

        rb = GetComponent<Rigidbody>();
        camPanTilt = GetComponent<CinemachinePanTilt>();
        swimFilter = GetComponent<AudioLowPassFilter>();
        swim = GetComponent<AudioSource>();
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
        if(transform.position.y > WaterY) rb.AddForce(Vector3.down * 0.5f);
        else rb.AddForce(Vector3.up * 0.2f);

        if (transform.position.y < WaterY - 0.1f)
        {
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, seaFog, Time.deltaTime);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.1f, Time.deltaTime);
            underwater.volume = Mathf.Lerp(underwater.volume, 0.5f, Time.deltaTime * 2f);
            seaWave.volume = Mathf.Lerp(seaWave.volume, 0.1f, Time.deltaTime * 2f);
            swimFilter.cutoffFrequency = 2000f;
        }
        else
        {
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.gray, Time.deltaTime * 5f);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.01f, Time.deltaTime * 5f);
            underwater.volume = Mathf.Lerp(underwater.volume, 0f, Time.deltaTime * 5f);
            seaWave.volume = Mathf.Lerp(seaWave.volume, 0.7f, Time.deltaTime * 5f);
            swimFilter.cutoffFrequency = 5007.7f;
        }

        if(forward > 0.1f) swim.volume = 1f;
        else swim.volume = 0.05f;
    }
}
