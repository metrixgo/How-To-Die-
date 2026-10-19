using System.Collections;
using UnityEngine;

public class DriveScene : MonoBehaviour
{
    private Vector3 offset = new Vector3(-284.201233f, -1.17336345f, -5.17994452f);

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone dies before they even realize it."));
        StartCoroutine(GameManager.Instance.ExitCutScene(3f));
    }
}