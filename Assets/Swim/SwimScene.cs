using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SwimScene : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] private Volume volume;
    [SerializeField] private Image screen;
    [SerializeField] private GameObject focus;
    private DepthOfField dof;
    private Vignette v;

    [Header("Dialogue")]
    [SerializeField] private GameObject dScreen;
    [SerializeField] private TextMeshProUGUI dContent;
    [SerializeField] private TextMeshProUGUI dSpeaker;
    [SerializeField] private AudioSource writingEffectsPlayer;

    private GameInput input;
    private InputAction skipAction;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        volume.profile.TryGet(out dof);
        volume.profile.TryGet(out v);

        input = new GameInput();
        skipAction = input.Game.Skip;
    }

    private IEnumerator Start()
    {
        /*yield return new WaitForSeconds(4f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone knows they¡¯re dying. But they can do nothing."));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("They can just watch death walk towards them. Step by step."));
        yield return new WaitForSeconds(1f);
        StartCoroutine(GameManager.Instance.ExitCutScene(3f));
        yield return new WaitForSeconds(2f);
        dScreen.SetActive(true);
        yield return StartCoroutine(DisplayDialogue("Brother", "Come on, it's not that far away!"));
        yield return StartCoroutine(DisplayDialogue("You", "But... I'm not feeling very well..."));
        yield return StartCoroutine(DisplayDialogue("Brother", "Ok fine, then I'm gonna swim to the lighthouse and tell everyone you were afraid to do so."));
        yield return StartCoroutine(DisplayDialogue("You", "Wait no! I'll go with you!"));
        yield return StartCoroutine(DisplayDialogue("Brother", "There we go! Go get your goggles upstairs, I'll meet you here."));
        yield return StartCoroutine(DisplayDialogue("You", "Alright..."));
        dScreen.SetActive(false);
        focus.SetActive(true);
        */
        yield return StartCoroutine(GameManager.Instance.ExitCutScene(0f));
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