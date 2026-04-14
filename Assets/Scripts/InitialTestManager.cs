using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
 
public class InitialTestManager : MonoBehaviour
{
    public static InitialTestManager Instance;
 
    [Header("UI")]
    public GameObject toastPanel;
    public GameObject DMarker;
    public GameObject EMarker;
    public Text toastText;
 
    [Header("Canvas")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Canvas targetCanvas2;
    [SerializeField] private float hideDelay = 5f;
 
    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip finalSound;
    public AudioClip resetSound;
 
    [Header("Toggles – Marcadores")]
    public Toggle miToggleA;
    public Toggle miToggleB;
    public Toggle miToggleC;
 
    [Header("Toggles – Movimientos")]
    public Toggle miToggleM;
    public Toggle miToggleRX;
    public Toggle miToggleRY;
 
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
 
    // ── Lógica interna ─────────────────────────────────────────────
 
    /// <summary>Procesa el mensaje, actualiza toggles/timer y devuelve el texto a mostrar.</summary>
    private string HandleMessage(string message)
    {
        switch (message)
        {
            // ── Marcadores A-B-C ──
            case "A":
                miToggleA.isOn = true;
                CheckTimer(0);
                return ResolveMarkerMessage("Se ha pasado por el marcador A");
 
            case "B":
                miToggleB.isOn = true;
                CheckTimer(0);
                return ResolveMarkerMessage("Se ha pasado por el marcador B");
 
            case "C":
                miToggleC.isOn = true;
                CheckTimer(0);
                return ResolveMarkerMessage("Se ha pasado por el marcador C");
 
            // ── Puntos de navegación ──
            case "D":
                DMarker.SetActive(false);
                audioSource.clip = successSound;
                return "Continúa con la prueba de acciones";
 
            case "E":
                EMarker.SetActive(false);
                audioSource.clip = successSound;
                return "Continúa con el test de cableado";
 
            // ── Movimientos M-RX-RY ──
            case "Movimiento":
                miToggleM.isOn = true;
                CheckTimer(1);
                return ResolveMovementMessage("Se ha realizado un movimiento con el objeto");
 
            case "Giro en X":
                miToggleRX.isOn = true;
                CheckTimer(1);
                return ResolveMovementMessage("Se ha realizado un giro en X con el objeto");
 
            case "Giro en Y":
                miToggleRY.isOn = true;
                CheckTimer(1);
                return ResolveMovementMessage("Se ha realizado un giro en Y con el objeto");
 
            // ── Reset ──
            case "Reset":
                audioSource.clip = resetSound;
                WireTestManager.Instance.ResetTest();
                ResetTest();
                return "El escenario ha sido reseteado";
 
            default:
                Debug.LogWarning($"[InitialTestManager] Mensaje desconocido: {message}");
                return string.Empty;
        }
    }
 
    /// <summary>Actualiza sonido y marcador D según si A-B-C están completos.</summary>
    private string ResolveMarkerMessage(string defaultMsg)
    {
        bool allDone = MarkersComplete();
        audioSource.clip = allDone ? finalSound : successSound;
        DMarker.SetActive(allDone);
        return allDone ? "¡Felicidades, ve al nuevo punto D!" : defaultMsg;
    }
 
    /// <summary>Actualiza sonido y marcador E según si M-RX-RY están completos.</summary>
    private string ResolveMovementMessage(string defaultMsg)
    {
        bool allDone = MovementsComplete();
        audioSource.clip = allDone ? finalSound : successSound;
        EMarker.SetActive(allDone);
        return allDone ? "¡Ve al nuevo punto E, para el test de cableado!" : defaultMsg;
    }
 
    private bool MarkersComplete()   => miToggleA.isOn  && miToggleB.isOn  && miToggleC.isOn;
    private bool MovementsComplete() => miToggleM.isOn  && miToggleRX.isOn && miToggleRY.isOn;
    private bool AllComplete()       => MarkersComplete() && MovementsComplete();
 
    /// <summary>Gestiona el timer para el grupo indicado (0 = marcadores, 1 = movimientos).</summary>
    private void CheckTimer(int group)
    {
        bool anyActive, allActive;
 
        if (group == 0)
        {
            anyActive = miToggleA.isOn  || miToggleB.isOn  || miToggleC.isOn;
            allActive = MarkersComplete();
        }
        else
        {
            anyActive = miToggleM.isOn  || miToggleRX.isOn || miToggleRY.isOn;
            allActive = MovementsComplete();
        }
 
        if (!timerRunning && anyActive)
        {
            timerValue   = 0f;
            timerRunning = true;
            Debug.Log("[InitialTestManager] Timer iniciado.");
        }
 
        if (timerRunning && allActive)
        {
            timerRunning = false;
            Debug.Log($"[InitialTestManager] Timer detenido: {timerValue:F2}s");
            SaveTimerToFile(timerValue, group);
        }
    }
 
    private void SaveTimerToFile(float timeValue, int group)
    {
        string path = Application.persistentDataPath + "/tiempos.txt";
        string tipo = group == 0 ? "Marcadores (A-B-C)" : "Movimientos (M-RX-RY)";
        string line  = $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss} | {tipo} | Tiempo: {timeValue:F2} segundos";
        File.AppendAllText(path, line + "\n");
        Debug.Log($"[InitialTestManager] Guardado en: {path}");
    }
 
    private void ResetTest()
    {
        miToggleA.isOn  = false;
        miToggleB.isOn  = false;
        miToggleC.isOn  = false;
        miToggleM.isOn  = false;
        miToggleRX.isOn = false;
        miToggleRY.isOn = false;
        timerRunning    = false;
        timerValue      = 0f;
        targetCanvas.gameObject.SetActive(true);
    }
 
    // ── Corrutinas ─────────────────────────────────────────────────
 
    private IEnumerator HideCanvasAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        targetCanvas.gameObject.SetActive(false);
        targetCanvas2.gameObject.SetActive(true);
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
