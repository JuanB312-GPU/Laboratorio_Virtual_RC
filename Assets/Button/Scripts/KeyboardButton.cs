using UnityEngine;
using TMPro;

public class KeyboardButton : MonoBehaviour
{

    KeyboardCLI keyboard;
    TextMeshProUGUI buttonText;
    void Start()
    {
        keyboard = GetComponentInParent<KeyboardCLI>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if(buttonText.text.Length == 1)
        {
            NameToButtonText();
            GetComponentInChildren<ButtonVR>().onRelease.AddListener(delegate { keyboard.InsertChar(buttonText.text); });
        }
    }

    public void NameToButtonText()
    {
        buttonText.text = gameObject.name;
    }
}
