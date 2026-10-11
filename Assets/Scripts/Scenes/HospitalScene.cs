using System.Collections;
using UnityEngine;

public class HospitalScene : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Mommy!"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Yes, honey?"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Mom... how do people die?"));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("......"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Mom...?"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Why are you asking this?"));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("You said people all die one day..."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("But I don't understand how people die..."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Well... this is a deep question..."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("I'm not sure how to describe it to you..."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Mom! Please? I want to know..."));
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Okay."));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("People die in different ways... because of different reasons."));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(GameManager.Instance.DisplayDialogue("Someone dies peacefully, without any struggle or resistance."));
        yield return StartCoroutine(GameManager.Instance.ExitCutScene(2f));
    }
}
