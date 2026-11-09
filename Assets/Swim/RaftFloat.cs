using UnityEngine;

public class RaftFloat : MonoBehaviour
{
    private float t = 0f;
    private float freq = 8f;
    private float amp = 0.1f;

    private Vector3 b;

    private void Awake()
    {
        b = transform.position;
    }

    private void Update()
    {
        t += Time.deltaTime;
        transform.position = new Vector3(
            b.x,
            b.y + Mathf.Sin(2f * Mathf.PI * t / freq) * amp,
            b.z
        );
    }
}
