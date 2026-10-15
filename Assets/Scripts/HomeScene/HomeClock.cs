using UnityEngine;

public class HomeClock : MonoBehaviour
{
    [SerializeField] private Transform secHand;
    [SerializeField] private Transform minHand;
    [SerializeField] private Transform hourHand;
    [SerializeField] private AudioClip tick;
    [SerializeField] private AudioClip tock;
    [SerializeField] private int seconds = 0;

    private float t = 0;
    private bool isTick = true;
    private AudioSource ad;

    private void Start()
    {
        ad = GetComponent<AudioSource>();
        secHand.rotation = Quaternion.Euler(secHand.rotation.x, secHand.rotation.y, seconds % 60 * 6);
        minHand.rotation = Quaternion.Euler(minHand.rotation.x, minHand.rotation.y, seconds / 60 * 6);
        hourHand.rotation = Quaternion.Euler(hourHand.rotation.x, hourHand.rotation.y, seconds / 3600 * 30);
    }

    private void Update()
    {
        t += Time.deltaTime;
        if (t >= 1.0f)
        {
            t -= 1.0f;
            if (isTick) ad.clip = tick;
            else ad.clip = tock;
            ad.Play();
            isTick = !isTick;
            seconds++;
            secHand.rotation = Quaternion.Euler(secHand.rotation.x, secHand.rotation.y, seconds % 60 * 6);
            minHand.rotation = Quaternion.Euler(minHand.rotation.x, minHand.rotation.y, seconds / 60 * 6);
            hourHand.rotation = Quaternion.Euler(hourHand.rotation.x, hourHand.rotation.y, seconds / 720 * 6);
        }
    }
}