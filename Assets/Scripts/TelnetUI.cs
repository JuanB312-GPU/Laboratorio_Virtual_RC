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
    var kb = Keyboard.current;
    bool enterPressed = kb != null &&
        (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame);

    if (!enterPressed) return;

    if (string.IsNullOrEmpty(text))
    {
        // ENTER vacío → nueva línea + prompt
        SendCommand("\r");
    }
    else
    {
        SendCommand(text + "\r");
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
    input = Regex.Replace(input, @"[\p{Cc}&&[^\n\t]]", string.Empty);
    // 4) Normalizar saltos de línea Telnet (CRLF / CR / LF → LF)
    input = input.Replace("\r\n", "\n");
    input = input.Replace("\r", "\n");

// 5) Colapsar múltiples saltos de línea en uno solo (opcional pero recomendado)
input = Regex.Replace(input, @"\n{3,}", "\n\n");
    return input;
    }

   public void AppendOutput(string text)
    {

    cliBuffer.Append(text);
string clean = CleanTelnetText(cliBuffer.ToString());
outputText.text = clean;

    // Hacer scroll al final
    Canvas.ForceUpdateCanvases();
    scrollRect.verticalNormalizedPosition = 0f;
    Canvas.ForceUpdateCanvases();
    }

    public void SendCommand(string rawCommand)
{
    bool isPureEnter = rawCommand == "\r" || rawCommand == "\n" || rawCommand == "\r\n";

    string command;

    if (isPureEnter) 
    {
        // ENTER real → no tocar
        command = "";
    }
    else
    {
        // Texto normal → limpiar saltos accidentales
        command = rawCommand.TrimEnd('\r', '\n');
    }

    // Limitar tamaño del buffer
    if (cliBuffer.Length > maxOutputChars)
    {
        cliBuffer.Remove(0, cliBuffer.Length - maxOutputChars);
    }

    outputText.text = cliBuffer.ToString();

    try
    {
        // IMPORTANTE: TelnetClient agregará CRLF
        telnetClient?.Send(command);
    }
    catch (System.Exception ex)
    {
        cliBuffer.AppendLine($"[Telnet] Error enviando: {ex.Message}");
        outputText.text = cliBuffer.ToString();
    }

    inputField.text = "";
    inputField.ActivateInputField();
}
    void Update()
    {
    if (!inputField.isFocused)
        inputField.ActivateInputField();
    }

}