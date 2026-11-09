using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReturnToPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player; 

    private GameInput input;
    private InputAction returnAction;

    private void Awake()
    {
        input = new GameInput();
        returnAction = input.Game.Return;
    }

    private void Update()
    {
        if (returnAction.WasPressedThisFrame())
        {
            player.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
