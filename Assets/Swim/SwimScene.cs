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
    [SerializeField] private Light sun;
    [SerializeField] private Renderer sea;
    [SerializeField] private AudioSource waves;
    [SerializeField] private AudioSource creep;
    private DepthOfField dof;
    private Vignette v;

    [Header("Dialogue")]
    [SerializeField] private GameObject focus;
    [SerializeField] private GameObject dScreen;
    [SerializeField] private TextMeshProUGUI dContent;
    [SerializeField] private TextMeshProUGUI dSpeaker;
    [SerializeField] private AudioSource writingEffectsPlayer;
    [SerializeField] private PlayerController player;

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

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private IEnumerator Start()
    {
        /*yield return new WaitForSeconds(4f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone knows they¡¯re dying. But they can do nothing."));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("They can just watch death walk towards them. Step by step."));
        yield return new WaitForSeconds(1f);
        focus.SetActive(false);
        player.CanDo.Move = false;
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
        player.CanDo.Move = true;
        */
        yield return StartCoroutine(GameManager.Instance.ExitCutScene(0f));
    }

    public void DarkenSky()
    {
        StartCoroutine(Dark());
    }

    private IEnumerator Dark()
    {
        float t = 0f, l = 6f, sSun = sun.intensity, sV = waves.volume, eV = 0.2f;
        Color sCol = RenderSettings.skybox.GetColor("_SkyColor"), eCol = new Color(0.192f, 0.231f, 0.278f);
        Color sSeaCol = sea.material.GetColor("Color_36218622185947c6a5ae36366d8e21d8"), eSeaCol = new Color(0.047f, 0.071f, 0.129f);

        while (t < l)
        {
            waves.volume = Mathf.Lerp(sV, eV, t / l);
            creep.volume = Mathf.Lerp(0f, 0.01f, t / l);
            RenderSettings.skybox.SetColor("_SkyColor", Color.Lerp(sCol, eCol, t / l));
            sea.material.SetColor("Color_36218622185947c6a5ae36366d8e21d8", Color.Lerp(sSeaCol, eSeaCol, t / l));
            t += Time.deltaTime;
            sun.intensity = Mathf.Lerp(sSun, 0.1f, t / l);
            yield return null;
        }
        waves.volume = eV;
        creep.volume = 0.01f;
        RenderSettings.skybox.SetColor("_SkyColor", eCol);
        sea.material.SetColor("Color_36218622185947c6a5ae36366d8e21d8", eSeaCol);
        sun.intensity = 0.1f;
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