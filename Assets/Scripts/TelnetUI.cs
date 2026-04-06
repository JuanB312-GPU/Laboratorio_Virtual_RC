using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public class TelnetUI : MonoBehaviour
{
    public int maxOutputChars = 10000;

    public TelnetClient telnetClient;
    public TMP_Text outputText;
    public TMP_InputField inputField;
    public ScrollRect scrollRect;

    private StringBuilder cliBuffer   = new StringBuilder(); // texto crudo
    private StringBuilder cleanBuffer = new StringBuilder(); // texto ya limpio

    public static System.Action<bool> OnCapsChanged;
    private bool capsActive = false;

    // ✅ Regex precompilados — se crean una sola vez
    private static readonly Regex RxCsi    = new Regex(@"\x1B\[[0-9;?]*[@-~]", RegexOptions.Compiled);
    private static readonly Regex RxOsc    = new Regex(@"\x1B].[^\x1B]*\x07",  RegexOptions.Compiled);
    private static readonly Regex RxEsc    = new Regex(@"\x1B.",               RegexOptions.Compiled);
    private static readonly Regex RxCtrl   = new Regex(@"[\x00-\x08\x0B\x0C\x0E-\x1F]", RegexOptions.Compiled);
    private static readonly Regex RxCc     = new Regex(@"[\p{Cc}&&[^\n\t]]",  RegexOptions.Compiled);
    private static readonly Regex RxMultiN = new Regex(@"\n{3,}",              RegexOptions.Compiled);

    // ✅ Control del loop de refresco
    private bool _pendingUIUpdate = false;
    public float uiRefreshRate = 0.05f; // máximo 20 refrescos por segundo

    void Start() { }

    void OnEnable()
    {
        telnetClient.OnDataReceived.AddListener(AppendOutput);
        inputField.onEndEdit.AddListener(OnInputEndEdit);
        inputField.ActivateInputField();
        StartCoroutine(UIRefreshLoop());
    }

    void OnDisable()
    {
        telnetClient.OnDataReceived.RemoveListener(AppendOutput);
        inputField.onEndEdit.RemoveListener(OnInputEndEdit);
        StopAllCoroutines();
    }

    // ✅ Limpia solo el fragmento nuevo, no todo el buffer
    public void AppendOutput(string text)
    {
        cliBuffer.Append(text);

        string newClean = CleanTelnetText(text); // ← solo el texto nuevo
        cleanBuffer.Append(newClean);

        // Limitar tamaño de ambos buffers
        if (cliBuffer.Length > maxOutputChars)
        {
            int excess = cliBuffer.Length - maxOutputChars;
            cliBuffer.Remove(0, excess);

            if (excess < cleanBuffer.Length)
                cleanBuffer.Remove(0, excess);
            else
                cleanBuffer.Clear();
        }

        _pendingUIUpdate = true;
    }

    // ✅ Refresca la UI a 20fps máximo — cleanBuffer ya está listo, solo asigna
    IEnumerator UIRefreshLoop()
    {
        var wait = new WaitForSeconds(uiRefreshRate);
        while (true)
        {
            if (_pendingUIUpdate)
            {
                _pendingUIUpdate = false;
                outputText.SetText(cleanBuffer); // ← SetText evita allocations extra de TMP

                // Espera un frame para que el layout esté listo antes del scroll
                yield return null;
                scrollRect.verticalNormalizedPosition = 0f;
            }
            yield return wait;
        }
    }

    string CleanTelnetText(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // 1) Eliminar secuencias CSI/ANSI
        input = RxCsi.Replace(input, string.Empty);
        // 2) Eliminar OSC y secuencias ESC simples
        input = RxOsc.Replace(input, string.Empty);
        input = RxEsc.Replace(input, string.Empty);
        // 3) Remover caracteres de control no imprimibles
        input = RxCtrl.Replace(input, string.Empty);
        input = RxCc.Replace(input, string.Empty);
        // 4) Normalizar saltos de línea
        input = input.Replace("\r\n", "\n").Replace("\r", "\n");
        // 5) Colapsar múltiples saltos de línea
        input = RxMultiN.Replace(input, "\n\n");

        return input;
    }

    public void SendCommand(string rawCommand)
    {
        bool isPureEnter = rawCommand == "\r" || rawCommand == "\n" || rawCommand == "\r\n";
        string command = isPureEnter ? "" : rawCommand.TrimEnd('\r', '\n');

        // Limitar tamaño del buffer
        if (cliBuffer.Length > maxOutputChars)
        {
            int excess = cliBuffer.Length - maxOutputChars;
            cliBuffer.Remove(0, excess);

            if (excess < cleanBuffer.Length)
                cleanBuffer.Remove(0, excess);
            else
                cleanBuffer.Clear();
        }

        try
        {
            // IMPORTANTE: TelnetClient agregará CRLF
            telnetClient?.Send(command);
        }
        catch (System.Exception ex)
        {
            AppendOutput($"[Telnet] Error enviando: {ex.Message}\n");
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }

    void OnInputEndEdit(string text)
    {
        var kb = Keyboard.current;
        bool enterPressed = kb != null &&
            (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame);

        if (!enterPressed) return;

        if (string.IsNullOrEmpty(text))
            SendCommand("\r");
        else
            SendCommand(text + "\r");
    }

    void Update()
    {
        if (!inputField.isFocused)
            inputField.ActivateInputField();
    }

    // ── Teclado virtual (sin cambios) ──────────────────────────────────────

    public void VirtualKeyPress(string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        switch (key.ToLower())
        {
            case "caps":
                capsActive = !capsActive;
                OnCapsChanged?.Invoke(capsActive);
                return;

            case "entrar":
                if (string.IsNullOrEmpty(inputField.text))
                    SendCommand("\r");
                else
                    SendCommand(inputField.text + "\r");
                return;

            case "retroceso":
                if (inputField.text.Length > 0)
                    inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
                break;

            case "espacio":
                inputField.text += " ";
                break;

            case "←":
                if (inputField.caretPosition > 0)
                    inputField.caretPosition--;
                return;

            case "→":
                if (inputField.caretPosition < inputField.text.Length)
                    inputField.caretPosition++;
                return;

            default:
                string value = capsActive ? ApplyCaps(key) : key;
                inputField.text += value;
                break;
        }

        inputField.caretPosition = inputField.text.Length;
    }

    static string ApplyCaps(string value)
    {
        switch (value)
        {
            case "1": return "!";
            case "2": return "@";
            case "3": return "#";
            case "4": return "$";
            case "5": return "%";
            case "6": return "^";
            case "7": return "&";
            case "8": return "*";
            case "9": return "(";
            case "0": return ")";
        }
        return value.ToUpper();
    }

    public void ClearOutput()
    {
        cliBuffer.Clear();
        cleanBuffer.Clear();
    }
}