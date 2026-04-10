using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ManualCaret : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private RectTransform caretRect;

    private Image caretImage;
    private float blinkTimer = 0f;
    private float blinkRate = 1f; // segundos

    void Start()
    {
        caretImage = caretRect.GetComponent<Image>();

        // Ocultar caret original
        inputField.caretWidth = 0;
        inputField.caretBlinkRate = 0;
    }

    void LateUpdate()
    {
        caretRect.gameObject.SetActive(true);

        inputField.ForceLabelUpdate(); // 👈 clave en Quest
        inputField.onValueChanged.AddListener(_ => StartCoroutine(FixCaretNextFrame()));

        // Parpadeo
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkRate)
        {
            blinkTimer = 0f;
            caretImage.enabled = !caretImage.enabled;
        }
    }

    void UpdateCaretPosition()
    {
        TMP_TextInfo textInfo = inputField.textComponent.textInfo;

        if (inputField.text.Length == 0)
        {
            // Sin texto: posición inicial izquierda
            Vector2 startPos = new Vector2(
                inputField.textComponent.rectTransform.rect.xMin + 2f,
                0f
            );
            caretRect.anchoredPosition = startPos;
            return;
        }

        int caretPos = Mathf.Clamp(
            inputField.caretPosition,
            0,
            textInfo.characterCount - 1
        );

        TMP_CharacterInfo charInfo = textInfo.characterInfo[caretPos];

        // Usa el origen del caracter actual como posición del caret
        Vector2 pos = new Vector2(charInfo.origin, charInfo.baseLine);
        caretRect.anchoredPosition = pos;
    }

    IEnumerator FixCaretNextFrame()
    {
        yield return null;
        UpdateCaretPosition();
    }

}