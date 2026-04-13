using UnityEngine;
using UnityEngine.UI;   // si usas botón de UI
// using UnityEngine.XR.Interaction.Toolkit.Interactables; // si usas botón XR

public class ResetManager : MonoBehaviour
{
    [Header("Objetos a resetear")]
    [SerializeField] private Resettable[] resettableObjects;
    [Header("Marcadores a resetear")]
    [SerializeField] private GameObject[] markerObjects;

    [Header("Botón (opcional si lo asignas por Inspector)")]
    [SerializeField] private Button resetButton;

    void Start()
    {
        // Si usas botón UI
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetAll);
    }

    public void ResetAll()
    {
        foreach (var obj in resettableObjects)
        {
            if (obj != null)
                obj.ResetState();
        }
        for (int i = 0; i < markerObjects.Length; i++)
        {
            if (markerObjects[i] != null)
                markerObjects[i].SetActive(true); // Reactiva los marcadores
        }

        ToastManager.Instance.ShowToast("Reset");
    }
}