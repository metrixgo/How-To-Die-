using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingScene : MonoBehaviour
{
    [Header("Rendering")]
    [SerializeField] private Image frontScreen;
    [SerializeField] private CanvasGroup ask;
    [SerializeField] private CanvasGroup sleep;

    private Color startScreenColor = new Color(0f, 0f, 0f, 50f / 255f);

    private void Start()
    {
        frontScreen.color = startScreenColor;
    }

    public void Ask()
    {
        StartCoroutine(StartGame());
    }

    public void Sleep()
    {
        StartCoroutine(QuitGame());
    }

    private IEnumerator StartGame()
    {
        GameManager.Instance.EnableRaycast(false);
        StartCoroutine(HideButtons());

        float t = 0f, l = 3f;
        while (t < l)
        {
            GameManager.Instance.SetMusic(Mathf.Lerp(1f, GameManager.Instance.cutSceneVolume, t / l));
            frontScreen.color = Color.Lerp(startScreenColor, Color.clear, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        GameManager.Instance.StartGame();
        SceneManager.LoadScene("HospitalScene");
    }

    private IEnumerator QuitGame()
    {
        GameManager.Instance.EnableRaycast(false);
        StartCoroutine(HideButtons());

        float t = 0f, l = 8f;
        while (t < l)
        {
            GameManager.Instance.SetMusic(Mathf.Lerp(1f, 0f, t / l));
            GameManager.Instance.SetEye(Mathf.Lerp(0f, 1f, t / l));
            frontScreen.color = Color.Lerp(startScreenColor, Color.black, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        Application.Quit();
    }

    private IEnumerator HideButtons()
    {
        float t = 0f, l = 1f;
        while (t < l)
        {
            ask.alpha = Mathf.Lerp(1f, 0f, t / l);
            sleep.alpha = Mathf.Lerp(1f, 0f, t / l);

            t += Time.deltaTime;
            yield return null;
        }

        ask.gameObject.SetActive(false);
        sleep.gameObject.SetActive(false);
    }

}