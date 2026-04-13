using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class WireTestManager : MonoBehaviour
{
    public static WireTestManager Instance;

    [Header("UI Reference")]
    public GameObject toastPanel; 
    public GameObject FMarker;
    public Text toastText; // or TextMeshProUGUI if using TMP
    [Header("Canvas a ocultar")]
    [SerializeField] private Canvas targetCanvas;
    [Header("Tiempo antes de ocultar")]
    [SerializeField] private float hideDelay = 5f;


    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip finalSound;

    public AudioClip resetSound;
    public Toggle miToggleCA;
    public Toggle miToggleCB;
    public Toggle miToggleW;

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

    private void CheckTimer()
    {
        bool timerEnable = false;

        timerEnable = miToggleCA.isOn && miToggleCB.isOn && miToggleW.isOn;

        if (!timerRunning && (miToggleCA.isOn || miToggleCB.isOn || miToggleW.isOn))
        {
             // Arranca cuando se activa el primero
             timerValue = 0f;
             timerRunning = true;
             Debug.Log("Timer started");
        }

        if (timerRunning && timerEnable)
        {
            // Se detiene cuando los tres están activos
            timerRunning = false;
            Debug.Log("Timer stopped: " + timerValue.ToString("F2") + " seconds");
            SaveTimerToFile(timerValue, 2);
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

        switch (message)
        {
            case "Cube1":
                miToggleCA.isOn = true;
                completeMessage = "Se ha puesto el primer elemento en el rack";
                CheckTimer();
                break;
            case "Cube2":
                miToggleCB.isOn = true;
                completeMessage = "Se ha puesto el segundo elemento en el rack";
                CheckTimer();
                break;
            case "Wire":
                miToggleW.isOn = true;
                completeMessage = "Se ha hecho la conexión por consola";
                CheckTimer();
                break;
            case "F":
                FMarker.SetActive(false);
                completeMessage = "Empieza el ejercicio con el CLI";
                break;
        }

            enable1 = miToggleCA.isOn && miToggleCB.isOn && miToggleW.isOn;
            audioSource.clip = enable1 ? finalSound : successSound;
            if (enable1)
                completeMessage = "¡Ve al punto F de la consola!";
            FMarker.SetActive(enable1);


        // Verificar si TODAS las funciones están completas
        bool allComplete = miToggleCA.isOn && miToggleCB.isOn && miToggleW.isOn;

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

        string tipo = "Ejercicio de cableado";
        string line = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    " | " + tipo +
                    " | Tiempo: " + timeValue.ToString("F2") + " segundos";

        File.AppendAllText(path, line + "\n");

        Debug.Log("Guardado en: " + path);
    }

    private void resetTest()
    {
        miToggleCA.isOn = false;
        miToggleCB.isOn = false;
        miToggleW.isOn = false;
        timerRunning = false;
        timerValue = 0f;
        targetCanvas.gameObject.SetActive(false);
    }
}
