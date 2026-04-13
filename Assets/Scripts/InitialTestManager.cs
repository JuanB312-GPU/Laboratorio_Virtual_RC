using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class InitialTestManager : MonoBehaviour
{
    public static InitialTestManager Instance;

    [Header("UI Reference")]
    public GameObject toastPanel;
    public GameObject DMarker;
    public GameObject EMarker;
    public Text toastText; // or TextMeshProUGUI if using TMP
    [Header("Canvas a ocultar")]
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private Canvas targetCanvas2;
    [Header("Tiempo antes de ocultar")]
    [SerializeField] private float hideDelay = 5f;


    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip finalSound;

    public AudioClip resetSound;
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

    [Header("Timer")]
    private float timerValue = 0f;
    private bool timerRunning = false;

    void Update()
    {
        if (timerRunning)
            timerValue += Time.deltaTime;
    }

    private void CheckTimer(int caseValue)
    {
        bool timerEnable = false;
        switch (caseValue)
        {
            case 0:
                timerEnable = miToggleA.isOn && miToggleB.isOn && miToggleC.isOn;
                
                if (!timerRunning && (miToggleA.isOn || miToggleB.isOn || miToggleC.isOn))
                {
                    // Arranca cuando se activa el primero
                    timerValue = 0f;
                    timerRunning = true;
                    Debug.Log("Timer started");
                }
                break;
            case 1:
                timerEnable = miToggleM.isOn && miToggleRX.isOn && miToggleRY.isOn;

                if (!timerRunning && (miToggleM.isOn || miToggleRX.isOn || miToggleRY.isOn))
                {
                    // Arranca cuando se activa el primero
                    timerValue = 0f;
                    timerRunning = true;
                    Debug.Log("Timer started");
                }
                break;
        }

        if (timerRunning && timerEnable)
        {
            // Se detiene cuando los tres están activos
            timerRunning = false;
            Debug.Log("Timer stopped: " + timerValue.ToString("F2") + " seconds");
            SaveTimerToFile(timerValue, caseValue);
        }
    }

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
        bool enable1 = false;
        bool enable2 = false;

        switch (message)
        {
            case "A":
                miToggleA.isOn = true;
                completeMessage = "Se ha pasado por el marcador A";
                CheckTimer(0);
                break;
            case "B":
                miToggleB.isOn = true;
                completeMessage = "Se ha pasado por el marcador B";
                CheckTimer(0);
                break;
            case "C":
                miToggleC.isOn = true;
                completeMessage = "Se ha pasado por el marcador C";
                CheckTimer(0);
                break;
            case "D":
                completeMessage = "Continua con la prueba de acciones";
                DMarker.SetActive(false);
                break;
            case "E":
                completeMessage = "Continua con el test de cableado";
                EMarker.SetActive(false);
                break;
            case "Movimiento":
                miToggleM.isOn = true;
                completeMessage = "Se ha realizado un movimiento con el objeto";
                CheckTimer(1);
                break;
            case "Giro en X":
                miToggleRX.isOn = true;
                completeMessage = "Se ha realizado un giro en X con el objeto";
                CheckTimer(1);
                break;
            case "Giro en Y":
                miToggleRY.isOn = true;
                completeMessage = "Se ha realizado un giro en Y con el objeto";
                CheckTimer(1);
                break;
            case "Reset":
                completeMessage = "El escenario ha sido reseteado";
                audioSource.clip = resetSound;
                resetTest();
                break;
        }

        // Lógica de sonido para toggles de movimiento y rotación
        if (message == "Movimiento" || message == "Giro en X" || message == "Giro en Y")
        {
            enable1 = miToggleM.isOn && miToggleRX.isOn && miToggleRY.isOn;
            audioSource.clip = enable1 ? finalSound : successSound;
            if (enable1)
                completeMessage = "¡Ve al test de cableado!";
            EMarker.SetActive(enable1);
        }

        // Lógica de sonido para toggles de puntos de control
        if (message == "A" || message == "B" || message == "C")
        {
            enable2 = miToggleA.isOn && miToggleB.isOn && miToggleC.isOn;
            audioSource.clip = enable2 ? finalSound : successSound;
            if (enable2)
                completeMessage = "¡Felicidades, ve al nuevo punto D!";
            DMarker.SetActive(enable2);
        }

        // Verificar si TODAS las funciones están completas
        bool allComplete = miToggleA.isOn && miToggleB.isOn && miToggleC.isOn
                        && miToggleM.isOn && miToggleRX.isOn && miToggleRY.isOn;

        toastText.text = completeMessage;
        toastPanel.SetActive(true);
        audioSource.Play();
        StartCoroutine(AnimateToast());

        if (allComplete)
        {
            // Terminó todo — ocultar canvas después del delay
            StartCoroutine(HideCanvasAfterDelay());
        }
    }

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

    // Función para guardar los tiempos en un archivo de texto
    void SaveTimerToFile(float timeValue, int caseValue)
    {
        string path = Application.persistentDataPath + "/tiempos.txt";

        string tipo = caseValue == 0 ? "Marcadores (A-B-C)" : "Movimientos (M-RX-RY)";
        string line = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    " | " + tipo +
                    " | Tiempo: " + timeValue.ToString("F2") + " segundos";

        File.AppendAllText(path, line + "\n");

        Debug.Log("Guardado en: " + path);
    }

    private void resetTest()
    {
        miToggleA.isOn = false;
        miToggleB.isOn = false;
        miToggleC.isOn = false;
        miToggleM.isOn = false;
        miToggleRX.isOn = false;
        miToggleRY.isOn = false;
        timerRunning = false;
        timerValue = 0f;
        targetCanvas.gameObject.SetActive(true);
    }
}
