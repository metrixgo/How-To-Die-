using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SwimScene : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] private Volume volume;
    [SerializeField] private Image screen;
    private DepthOfField dof;
    private Vignette v;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        volume.profile.TryGet(out dof);
        volume.profile.TryGet(out v);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone knows they¡¯re dying. But they can do nothing."));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("They can just watch death walk towards them. Step by step."));
        yield return new WaitForSeconds(1f);
        StartCoroutine(GameManager.Instance.ExitCutScene(3f));
    }

}