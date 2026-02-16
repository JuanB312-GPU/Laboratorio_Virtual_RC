using UnityEngine;
using TMPro;

public class ChangeFontController : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TMP_Text CliText;
    public TMP_Text InputText;
    public TMP_FontAsset LiberationSans;
    public TMP_FontAsset Camingo;
    public TMP_FontAsset Inconsolata;
    public TMP_FontAsset CourierPrime;

    void Start()
    {
        OnDropdownChanged(dropdown.value);
    }

    public void OnDropdownChanged(int index)
    {

        switch (index)
        {
            case 0: // Liberations Sans
                CliText.font = LiberationSans;
                InputText.font = LiberationSans;
                break;

            case 1: // Camingo
                CliText.font = Camingo;
                InputText.font = Camingo;
                break;

            case 2: // Inconsolata
                CliText.font = Inconsolata;
                InputText.font = Inconsolata;
                break;

            case 3: // Courier Prime
                CliText.font = CourierPrime;
                InputText.font = CourierPrime;
                break;
        }
    }
}