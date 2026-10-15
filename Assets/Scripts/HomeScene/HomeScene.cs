using System.Collections;
using UnityEngine;

public class HomeScene : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone dies, because they used up all their time."));
        StartCoroutine(GameManager.Instance.ExitCutScene(3f));
    }
}
