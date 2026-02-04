using UnityEngine;

public class TerminalToggle : MonoBehaviour
{
    [Header("Referencias")]
    public Canvas terminalCanvas;  // Arrastra aquí el Canvas del terminal en el Inspector
    
    [Header("Configuración")]
    public KeyCode toggleKey = KeyCode.T;  // Tecla para mostrar/ocultar (T por Terminal)
    
    void Start()
    {
        // Ocultar el terminal al inicio
        if (terminalCanvas != null)
        {
            terminalCanvas.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        // Detectar tecla presionada
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleTerminal();
        }
    }
    
    void ToggleTerminal()
    {
        if (terminalCanvas != null)
        {
            // Alternar estado activo/inactivo
            bool newState = !terminalCanvas.gameObject.activeSelf;
            terminalCanvas.gameObject.SetActive(newState);
            
            Debug.Log(newState ? "Terminal abierto" : "Terminal cerrado");
        }
    }
    
    // Método público para llamar desde botones
    public void ShowTerminal()
    {
        if (terminalCanvas != null)
        {
            terminalCanvas.gameObject.SetActive(true);
        }
    }
    
    public void HideTerminal()
    {
        if (terminalCanvas != null)
        {
            terminalCanvas.gameObject.SetActive(false);
        }
    }
}
