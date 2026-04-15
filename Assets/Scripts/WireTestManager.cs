using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
 
public class WireTestManager : MonoBehaviour
{
    public static WireTestManager Instance;
 
    [Header("UI")]
    public GameObject toastPanel;
    public GameObject FMarker;
    public Text toastText;

    public GameObject CLIPanel;
    public GameObject DevicesPanel;
    public GameObject HelpPanel;

    public GameObject Keyboard;
 
    [Header("Canvas")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private float hideDelay = 5f;
 
    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip finalSound;
    public AudioClip resetSound;
 
    [Header("Toggles")]
    public Toggle miToggleCA;
    public Toggle miToggleCB;
    public Toggle miToggleW;
 
    [Header("Ajustes")]
    public float duration = 3f;
    public float fadeSpeed = 1f;
 
    private CanvasGroup canvasGroup;
    private float timerValue;
    private bool timerRunning;
 
    // ── Ciclo de vida ──────────────────────────────────────────────
 
    void Awake()
    {
        Instance = this;
        canvasGroup = toastPanel.GetComponent<CanvasGroup>();
        toastPanel.SetActive(false);
    }
 
    void Update()
    {
        if (timerRunning)
            timerValue += Time.deltaTime;
    }
 
    // ── API pública ────────────────────────────────────────────────
 
    public void ShowToast(string message)
    {
        StopAllCoroutines();
 
        string msg = HandleMessage(message);
 
        toastText.text = msg;
        toastPanel.SetActive(true);
        audioSource.Play();
        StartCoroutine(AnimateToast());
 
        if (AllComplete())
            StartCoroutine(HideCanvasAfterDelay());
    }
 
    public void ResetTest()
    {
        miToggleCA.isOn = false;
        miToggleCB.isOn = false;
        miToggleW.isOn = false;
        timerRunning = false;
        HelpPanel.SetActive(false);
        CLIPanel.SetActive(false);
        DevicesPanel.SetActive(false);
        Keyboard.SetActive(false);
        timerValue = 0f;
        targetCanvas.gameObject.SetActive(false);
    }
 
    // ── Lógica interna ─────────────────────────────────────────────
 
    /// <summary>Procesa el mensaje entrante, actualiza estado y devuelve el texto a mostrar.</summary>
    private string HandleMessage(string message)
    {
        switch (message)
        {
            case "Cube1":
                miToggleCA.isOn = true;
                CheckTimer();
                return ResolveRackMessage("Se ha puesto el primer elemento en el rack");
 
            case "Cube2":
                miToggleCB.isOn = true;
                CheckTimer();
                return ResolveRackMessage("Se ha puesto el segundo elemento en el rack");
 
            case "RJ45":
                miToggleW.isOn = true;
                CheckTimer();
                return ResolveRackMessage("Se ha conectado el cable a la consola");
 
            case "F":
                FMarker.SetActive(false);
                audioSource.clip = successSound;
                HelpPanel.SetActive(true);
                CLIPanel.SetActive(true);
                DevicesPanel.SetActive(true);
                Keyboard.SetActive(true);
                return "Empieza el ejercicio con el CLI";
 
            default:
                Debug.LogWarning($"[WireTestManager] Mensaje desconocido: {message}");
                return string.Empty;
        }
    }
 
    /// <summary>Devuelve el mensaje correcto según si el rack está completo o no,
    /// y actualiza el sonido y el marcador F.</summary>
    private string ResolveRackMessage(string defaultMsg)
    {
        bool allDone = AllComplete();
        audioSource.clip = allDone ? finalSound : successSound;
        FMarker.SetActive(allDone);
        return allDone ? "¡Ve al punto F de la consola!" : defaultMsg;
    }
 
    private bool AllComplete() =>
        miToggleCA.isOn && miToggleCB.isOn && miToggleW.isOn;
 
    /// <summary>Arranca el timer con el primer toggle activo; lo detiene cuando los tres lo están.</summary>
    private void CheckTimer()
    {
        bool anyActive  = miToggleCA.isOn || miToggleCB.isOn || miToggleW.isOn;
        bool allActive  = AllComplete();
 
        if (!timerRunning && anyActive)
        {
            timerValue   = 0f;
            timerRunning = true;
            Debug.Log("[WireTestManager] Timer iniciado.");
        }
 
        if (timerRunning && allActive)
        {
            timerRunning = false;
            Debug.Log($"[WireTestManager] Timer detenido: {timerValue:F2}s");
            SaveTimerToFile(timerValue);
        }
    }
 
    private void SaveTimerToFile(float timeValue)
    {
        string path = Application.persistentDataPath + "/tiempos.txt";
        string line  = $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss} | Ejercicio de cableado | Tiempo: {timeValue:F2} segundos";
        File.AppendAllText(path, line + "\n");
        Debug.Log($"[WireTestManager] Guardado en: {path}");
    }
 
    // ── Corrutinas ─────────────────────────────────────────────────
 
    private IEnumerator HideCanvasAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        targetCanvas.gameObject.SetActive(false);
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