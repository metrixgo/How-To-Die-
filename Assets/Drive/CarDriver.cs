using UnityEngine;
using UnityEngine.InputSystem;

public class CarDriver : MonoBehaviour
{
    [SerializeField] private GameObject focus;
    [SerializeField] private LayerMask ignoreLayer;

    private GameInput input;
    private InputAction interactAction;

    private Interactable curItem;
    private Interactable newItem;

    private void Awake()
    {
        input = new GameInput();
        interactAction = input.Player.Interact;
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
        if (!focus.activeSelf) return;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.5f, ~ignoreLayer))
        {
            newItem = hit.collider.GetComponentInParent<Interactable>();

            if (newItem == null)
            {
                if (curItem != null) curItem.SetFocused(false);

                curItem = null;
            }
            else
            {
                if (curItem != null && curItem != newItem)
                    curItem.SetFocused(false);

                curItem = newItem;

                if (curItem != null)
                    curItem.SetFocused(true);
            }
        }
        else
        {
            if (curItem != null) curItem.SetFocused(false);
            newItem = null;
            curItem = null;
        }

        if (interactAction.WasPressedThisFrame() && curItem != null)
            curItem.Interact();
    }
}