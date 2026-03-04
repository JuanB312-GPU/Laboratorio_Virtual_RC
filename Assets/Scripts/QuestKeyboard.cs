using UnityEngine;

public class QuestKeyboard : MonoBehaviour
{
    private TouchScreenKeyboard keyboard;

    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open(
            "",
            TouchScreenKeyboardType.Default,
            false,  // autocorrection
            false,  // multiline
            false,  // secure
            false   // alert
        );
    }

    void Update()
    {
        if (keyboard != null)
        {
            Debug.Log("Texto actual: " + keyboard.text);

            if (keyboard.status == TouchScreenKeyboard.Status.Done)
            {
                Debug.Log("Texto final: " + keyboard.text);
                keyboard = null;
            }
        }
    }
}
