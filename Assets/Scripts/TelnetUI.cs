using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public class TelnetUI : MonoBehaviour
{
    public int maxOutputChars = 50000; 

    public TelnetClient telnetClient;
    public TMP_Text outputText;
    public TMP_InputField inputField;
    public ScrollRect scrollRect;
    private StringBuilder cliBuffer = new StringBuilder();

    void Start()
    {
        
    }

     void OnEnable()
    {
        telnetClient.OnDataReceived.AddListener(AppendOutput);
        inputField.onEndEdit.AddListener(OnInputEndEdit);
        inputField.ActivateInputField(); // dar foco al abrir
    }

    void OnDisable()
    {
        telnetClient.OnDataReceived.RemoveListener(AppendOutput);
        inputField.onEndEdit.RemoveListener(OnInputEndEdit);
    }

    void OnInputEndEdit(string text)
    {
        // Si no hay teclado disponible (por ejemplo en algunos dispositivos), protegemos con null-check
        var kb = Keyboard.current;
        bool enterPressed = kb != null && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame);

        if (enterPressed)
            {
                SendCommand(text);
            }
        else
            {
                // Se disparó OnEndEdit por pérdida de foco: no enviamos
            }
    }

    string CleanTelnetText(string input)
    {
    if (string.IsNullOrEmpty(input)) return input;

    // 1) Eliminar secuencias CSI/ANSI que empiezan con ESC '[' y terminan en una letra (ej: \x1b[0m, \x1b[2K)
    // Explicación: \x1B = ESC, \[ = '[', [0-9;?]* = parámetros, [@-~] = finalizador de control
    input = Regex.Replace(input, @"\x1B\[[0-9;?]*[@-~]", string.Empty);

    // 2) Eliminar otras secuencias ESC comenzando por ESC + any-char (p. ej. OSC, BEL), opcional
    input = Regex.Replace(input, @"\x1B].[^\x1B]*\x07", string.Empty); // OSC ... BEL
    input = Regex.Replace(input, @"\x1B.", string.Empty); // secuencias simples con ESC

    // 3) Remover caracteres de control no imprimibles (0x00 - 0x1F) excepto \r(13), \n(10), \t(9)
    input = Regex.Replace(input, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty);

    return input;
    }

   public void AppendOutput(string text)
    {
    string clean = CleanTelnetText(text);
    cliBuffer.AppendLine(clean);
    outputText.text = cliBuffer.ToString();

    // Hacer scroll al final
    Canvas.ForceUpdateCanvases();
    scrollRect.verticalNormalizedPosition = 0f;
    Canvas.ForceUpdateCanvases();
    }

    public void SendCommand(string rawCommand)
    {
        if (string.IsNullOrWhiteSpace(rawCommand))
        {
            // Nada que enviar: mantener foco para seguir escribiendo
            if (inputField != null) inputField.ActivateInputField();
            return;
        }

        // Normalizar la linea (quitar saltos accidentales)
        string command = rawCommand.TrimEnd('\r', '\n');

        // Limitar tamaño del buffer para evitar memoria infinita
        if (cliBuffer.Length > maxOutputChars)
        {
            cliBuffer.Remove(0, cliBuffer.Length - maxOutputChars);
        }

        // Volcar buffer al TextMeshPro
        outputText.text = cliBuffer.ToString();

        

        // Enviar el comando por Telnet (TelnetClient añade el CRLF internamente)
        try
        {
            telnetClient?.Send(command);
        }
        catch (System.Exception ex)
        {
            // En caso de error al intentar enviar, lo reflejamos en la UI
            cliBuffer.AppendLine($"[Telnet] Error enviando: {ex.Message}");
            outputText.text = cliBuffer.ToString();
            
        }

        // Limpiar input y re-enfocar
        inputField.text = "";
        inputField.ActivateInputField();
    }

    void Update()
    {
    if (!inputField.isFocused)
        inputField.ActivateInputField();
    }

}