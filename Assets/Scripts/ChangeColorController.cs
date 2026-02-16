using UnityEngine;
using TMPro;

public class ChangeColorController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TMP_Text CliText;
    public TMP_Text InputText;

    void Start()
    {
        OnDropdownChanged(dropdown.value);
    }

    public void OnDropdownChanged(int index)
    {

        switch (index)
        {
            case 0: // Naranja CLI
                CliText.color = new Color(1f, 0.55f, 0f);
                InputText.color = new Color(1f, 0.55f, 0f);
                break;

            case 1: // Cyan
                CliText.color = new Color(0f, 1f, 1f);
                InputText.color = new Color(0f, 1f, 1f);
                break;

            case 2: // Blanco
                CliText.color = Color.white;
                InputText.color = Color.white;
                break;

            case 3: // Verde CLI
                CliText.color = new Color(0f, 1f, 0.27f);
                InputText.color = new Color(0f, 1f, 0.27f);
                break;
        }
    }
}
