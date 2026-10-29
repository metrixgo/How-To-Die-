using System.Collections;
using UnityEngine;

public class CarDoor : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject driver;

    private bool opened = false;
    private bool opening = false;
    private AudioSource ad;

    private void Awake()
    {
        ad = GetComponent<AudioSource>();
    }

    public void InteractDoor()
    {
        if (opening) return;

        opened = !opened;
        opening = true;

        if (opened) StartCoroutine(OpenDoor());
        else StartCoroutine(CloseDoor());
    }
    
    private IEnumerator OpenDoor()
    {
        yield return null;
    }

    private IEnumerator CloseDoor()
    {
        yield return null;
    }
}
