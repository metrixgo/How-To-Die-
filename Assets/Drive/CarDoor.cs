using UnityEngine;

public class CarDoor : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject driver;

    public void InteractDoor()
    {
        Debug.Log("YAY");
    }
}
