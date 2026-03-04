using UnityEngine;
using TMPro;

public class KeyboardCLI : MonoBehaviour
{
     public TMP_InputField inputField;
    public GameObject normalButtons;
    public GameObject capsButtons;
    private bool caps;

    void Start()
    {
        caps = false;
    }

    public void InsertChar(string key)
    {
        inputField.text += key;
    }

    public void DeleteChar()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public void InsertSpace()
    {
        inputField.text += " ";
    }

    public void CapsPressed()
    {
        if (caps)
        {
            normalButtons.SetActive(true);
            capsButtons.SetActive(false);
            caps = false;
        }
        else
        {
            normalButtons.SetActive(false);
            capsButtons.SetActive(true);
            caps = true;
        }
    }

}
