using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    [Header("UI Reference")]
    public GameObject toastPanel;
    public Text toastText; // or TextMeshProUGUI if using TMP

    [Header("Settings")]
    public float duration = 3f;
    public float fadeSpeed = 1f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;
        canvasGroup = toastPanel.GetComponent<CanvasGroup>();
        toastPanel.SetActive(false);
    }

    public void ShowToast(string message)
    {
        StopAllCoroutines();
        toastText.text = message;
        toastPanel.SetActive(true);
        StartCoroutine(AnimateToast());
    }

    private IEnumerator AnimateToast()
    {
        // Fade in
        canvasGroup.alpha = 0f;
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Wait
        yield return new WaitForSeconds(duration);

        // Fade out
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        toastPanel.SetActive(false);
    }
}
