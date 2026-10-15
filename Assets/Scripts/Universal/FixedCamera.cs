using Unity.Cinemachine;
using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    public float sensitivity = 10f;

    private InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller camX;
    private InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller camY;

    private void Awake()
    {
        CinemachineInputAxisController camController = GetComponent<CinemachineInputAxisController>();

        foreach (InputAxisControllerBase<CinemachineInputAxisController.Reader>.Controller controller in camController.Controllers)
        {
            if (controller.Name == "Look X (Pan)") camX = controller;
            else if (controller.Name == "Look Y (Tilt)") camY = controller;
            else Debug.LogError("Unknown Cinemachine Controller Name: " + controller.Name);
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.InCutScene)
        {
            camX.Input.Gain = sensitivity;
            camY.Input.Gain = -sensitivity;
        }
        else
        {
            camX.Input.Gain = 0;
            camY.Input.Gain = 0;
        }
    }
}
