using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class BrotherSwim : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private GameObject focus;
    [SerializeField] private GameObject dScreen;
    [SerializeField] private TextMeshProUGUI dContent;
    [SerializeField] private TextMeshProUGUI dSpeaker;
    [SerializeField] private AudioSource writingEffectsPlayer;

    public bool gotGoggles = false;

    private GameInput input;
    private InputAction skipAction;

    private Animator anim;
    private SplineAnimate sa;

    private bool asking = false;

    private void Awake()
    {
        input = new GameInput();
        skipAction = input.Game.Skip;

        anim = GetComponent<Animator>();
        sa = GetComponent<SplineAnimate>();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    public void Ask()
    {
        if(!asking) StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        asking = true;
        dScreen.SetActive(true);
        focus.SetActive(false);
        if (!gotGoggles)
        {
            yield return StartCoroutine(DisplayDialogue("Brother", "Go get your goggles upstairs!!! It's very dangerous to swim without it!!!"));
            yield return StartCoroutine(DisplayDialogue("You", "Alright..."));
            asking = false;
        }
        else
        {
            yield return StartCoroutine(DisplayDialogue("Brother", "Looks like you're ready!"));
            yield return StartCoroutine(DisplayDialogue("Brother", "Follow me closely. I don't want you to get lost... hehe."));
            yield return StartCoroutine(DisplayDialogue("You", "Sure..."));
        }
        dScreen.SetActive(false);
        focus.SetActive(true);

        if (gotGoggles)
        {
            anim.SetInteger("State", 1);
            float t = 0f, l = 5f, end = sa.Container.Spline.ConvertIndexUnit(5, PathIndexUnit.Knot, PathIndexUnit.Normalized);
            while (t < l)
            {
                t += Time.deltaTime;
                sa.NormalizedTime = Mathf.Lerp(0f, end, t / l);
            }
        }
    }

    private IEnumerator DisplayDialogue(string speaker, string content)
    {
        yield return new WaitForSeconds(0.05f);
        writingEffectsPlayer.Play();
        dSpeaker.text = speaker;

        float t = 0f, gap = 0.05f;
        int idx = 0;

        dContent.text = "";
        while (idx < content.Length)
        {
            t += Time.deltaTime;
            if (t >= gap)
            {
                t -= gap;
                dContent.text += content[idx++];
            }
            if (skipAction.WasPressedThisFrame())
            {
                dContent.text = content;
                break;
            }
            yield return null;
        }

        writingEffectsPlayer.Stop();
        dContent.text = content;

        yield return new WaitForSeconds(0.05f);
        yield return new WaitUntil(() => skipAction.WasPressedThisFrame());
    }
}
