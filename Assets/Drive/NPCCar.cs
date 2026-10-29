using UnityEngine;
using UnityEngine.Splines;

public class NPCCar : MonoBehaviour
{
    private SplineAnimate sa;

    private bool isActive = false;
    private float speed = 0.1f;
    private float curPos = 0f;

    private bool respawning = false;
    private float countDown = 0f;

    private void Start()
    {
        sa = GetComponent<SplineAnimate>();
    }

    private void Update()
    {
        if (!isActive)
        {
            if (!respawning)
            {
                respawning = true;
                countDown = Random.Range(0f, 2f);
                speed = Random.Range(0.06f, 0.14f);
                curPos = 0f;
            }
            else
            {
                countDown -= Time.deltaTime;
                if (countDown < 0f)
                {
                    isActive = true;
                    respawning = false;
                }
            }
        }
        else
        {
            curPos += speed * Time.deltaTime;
            sa.NormalizedTime = Mathf.Clamp01(curPos);

            if (curPos < 0 || curPos > 1) isActive = false;
        }
    }
}
