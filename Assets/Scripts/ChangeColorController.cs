using UnityEngine;
using TMPro;

public class ChangeColorController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TMP_Text targetText;

    void Start()
    {
        OnDropdownChanged(dropdown.value);
    }

    public void OnDropdownChanged(int index)
    {

        switch (index)
        {
            case 0: // Naranja CLI
                targetText.color = new Color(1f, 0.55f, 0f);
                break;

            case 1: // Cyan
                targetText.color = new Color(0f, 1f, 1f);
                break;

            case 2: // Blanco
                targetText.color = Color.white;
                break;

            case 3: // Verde CLI
                targetText.color = new Color(0f, 1f, 0.27f);
                break;
        }
    }
}
