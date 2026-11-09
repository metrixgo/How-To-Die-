using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FocusOnObject : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GameObject focusCamera;
    [SerializeField] private GameObject player;

    [Header("Settings")]
    [SerializeField] private float transitionLength = 1f;
    [SerializeField] private bool enableMouse = true;

    [Header("Rendering")]
    [SerializeField] private RenderTexture gameRenderTexture;
    [SerializeField] private GameObject focus;

    [Header("IgnoreLayer")]
    [SerializeField] private LayerMask ignoreLayer;

    [Header("Focus Events")]
    [SerializeField] private UnityEvent focusOnEvent;
    [SerializeField] private UnityEvent focusOffEvent;

    private CinemachineBlendDefinition blendStyle;

    private GameInput input;
    private InputAction returnAction;
    private InputAction interactAction;

    private bool focused = false;
    private bool transitioning = false;

    private void Awake()
    {
        input = new GameInput();
        returnAction = input.Game.Return;
        interactAction = input.Player.Interact;

        blendStyle = new CinemachineBlendDefinition(
            CinemachineBlendDefinition.Styles.EaseInOut,
            transitionLength
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
        if (returnAction.WasPressedThisFrame() && !transitioning && focused && focusCamera.activeSelf)
            FocusOff();
    }

    public void FocusOn()
    {
        if (!transitioning && !focused && player.activeSelf)
            StartCoroutine(Focus());
    }

    public void FocusOff()
    {
        if (!transitioning && focused && focusCamera.activeSelf)
            StartCoroutine(Focus());
    }

    public GameObject InteractedObject()
    {
        if (interactAction.WasPressedThisFrame() && enableMouse)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            float x = mousePos.x / Screen.width * gameRenderTexture.width;
            float y = mousePos.y / Screen.height * gameRenderTexture.height;
            Vector3 texPos = new Vector3(x, y, 0);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(texPos), out RaycastHit hit, Mathf.Infinity, ~ignoreLayer))
                return hit.collider.gameObject;
        }

        return null;
    }

    private IEnumerator Focus()
    {
        focused = !focused;
        focus.SetActive(false);

        transitioning = true;
        brain.DefaultBlend = blendStyle;
        focusCamera.SetActive(focused);
        player.SetActive(!focused);

        if (!focused)
        {
            focusOffEvent.Invoke();
            yield return new WaitForEndOfFrame();

            if (enableMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        yield return new WaitForSeconds(transitionLength);

        if (focused)
        {
            focusOnEvent.Invoke();

            if (enableMouse)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        transitioning = false;
        if (!focused) focus.SetActive(true);
    }
}
