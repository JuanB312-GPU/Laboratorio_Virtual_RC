using UnityEngine;
using UnityEngine.UI;

public class ButtonValueSetter : MonoBehaviour
{
    private string keyValue;
    private Text keyText;
    private TelnetUI telnetUI;

    void Start()
    {
        telnetUI = FindFirstObjectByType<TelnetUI>();

        string[] parts = gameObject.name.Split(' ');
        keyValue = parts[parts.Length - 1];

        keyText = GetComponentInChildren<Text>();

        UpdateVisual(false);

        GetComponent<Button>().onClick.AddListener(SendKey);

        TelnetUI.OnCapsChanged += UpdateVisual;
    }

    void OnDestroy()
    {
        TelnetUI.OnCapsChanged -= UpdateVisual;
    }

    void SendKey()
    {
        telnetUI?.VirtualKeyPress(keyValue);
    }

    void UpdateVisual(bool caps)
    {
        if (keyText == null) return;

        if (keyValue == "Espacio")
        {
            keyText.text = "";
            return;
        }

        keyText.text = caps ? ApplyCaps(keyValue) : keyValue;
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

        if (value == "Entrar" || value == "Retroceso" || value == "Caps")
        {
            return value;
        }
        else
        {
            return value.ToUpper();
        }

    }
}
