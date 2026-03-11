using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;

    [Header("UI Reference")]
    public GameObject toastPanel;
    public Text toastText; // or TextMeshProUGUI if using TMP

    public Toggle miToggleA;
    public Toggle miToggleB;
    public Toggle miToggleC;

    public Toggle miToggleM;
    public Toggle miToggleRX;
    public Toggle miToggleRY;

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
        string completeMessage = "";

        switch(message)
        {
            case "A":
                miToggleA.isOn = true;
                completeMessage = "Se ha pasado por el marcador A";
                break;
            case "B":
                miToggleB.isOn = true;
                completeMessage = "Se ha pasado por el marcador B";
                break;
            case "C":
                miToggleC.isOn = true;
                completeMessage = "Se ha pasado por el marcador C";
                break;
            case "Movimiento":
                miToggleM.isOn = true;
                completeMessage = "Se ha realizado la acción de movimiento";
                break;
            case "Rotación en X":
                miToggleRX.isOn = true;
                completeMessage = "Se ha realizado la acción de rotación en X";
                break;
            case "Rotación en Y":
                miToggleRY.isOn = true;
                completeMessage = "Se ha realizado la acción de rotación en Y";
                break;
        }
        toastText.text = completeMessage;
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
