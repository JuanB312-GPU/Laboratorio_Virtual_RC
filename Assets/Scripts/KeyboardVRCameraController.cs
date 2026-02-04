using UnityEngine;


public class XRSimulatorController : MonoBehaviour
{
    [Header("Referencias XR")]
    public Transform xrRig;
    public Transform mainCamera;
    public Transform leftController;
    public Transform rightController;
    
    [Header("Movimiento")]
    public float velocidadMovimiento = 3f;
    public float velocidadRotacion = 2f;
    
    [Header("Cámara")]
    public float sensibilidadMouse = 2f;
    public float limiteVertical = 80f;
    
    [Header("Controladores")]
    public float velocidadControladores = 2f;
    public float rangoMovimiento = 0.5f;
    
    private float rotacionVertical = 0f;
    private bool cursorBloqueado = true;
    private Vector3 posicionInicialIzq;
    private Vector3 posicionInicialDer;

    void Start()
    {
        // Bloquear cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Guardar posiciones iniciales de controladores
        if (leftController != null)
            posicionInicialIzq = leftController.localPosition;
        if (rightController != null)
            posicionInicialDer = rightController.localPosition;
    }

    void Update()
    {
        // Toggle cursor lock con ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorBloqueado = !cursorBloqueado;
            Cursor.lockState = cursorBloqueado ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !cursorBloqueado;
        }
        
        if (cursorBloqueado)
        {
            ManejarMovimientoRig();
            ManejarRotacionCamara();
        }
        
        ManejarControladores();
    }

    void ManejarMovimientoRig()
    {
        if (xrRig == null) return;
        
        // Movimiento WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 direccion = mainCamera.right * horizontal + mainCamera.forward * vertical;
        direccion.y = 0; // Mantener movimiento en plano horizontal
        
        xrRig.position += direccion.normalized * velocidadMovimiento * Time.deltaTime;
        
        // Rotación con Q y E
        if (Input.GetKey(KeyCode.Q))
            xrRig.Rotate(Vector3.up, -velocidadRotacion);
        if (Input.GetKey(KeyCode.E))
            xrRig.Rotate(Vector3.up, velocidadRotacion);
    }

    void ManejarRotacionCamara()
    {
        if (mainCamera == null) return;
        
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;
        
        // Rotación horizontal de la cámara
        mainCamera.Rotate(Vector3.up * mouseX, Space.World);
        
        // Rotación vertical de la cámara
        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -limiteVertical, limiteVertical);
        
        Vector3 rotacionActual = mainCamera.localEulerAngles;
        mainCamera.localEulerAngles = new Vector3(rotacionVertical, rotacionActual.y, 0f);
    }

    void ManejarControladores()
    {
        // Controlador Izquierdo - Teclas numéricas
        if (leftController != null)
        {
            Vector3 offset = Vector3.zero;
            
            if (Input.GetKey(KeyCode.Keypad4)) offset.x -= velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.Keypad6)) offset.x += velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.Keypad8)) offset.z += velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.Keypad5)) offset.z -= velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.Keypad7)) offset.y += velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.Keypad9)) offset.y -= velocidadControladores * Time.deltaTime;
            
            leftController.localPosition = Vector3.Lerp(
                leftController.localPosition,
                posicionInicialIzq + offset,
                Time.deltaTime * 5f
            );
            
            // Grip izquierdo - G
            if (Input.GetKeyDown(KeyCode.G))
                SimularGrip(leftController, true);
            if (Input.GetKeyUp(KeyCode.G))
                SimularGrip(leftController, false);
        }
        
        // Controlador Derecho - Teclas IJKL
        if (rightController != null)
        {
            Vector3 offset = Vector3.zero;
            
            if (Input.GetKey(KeyCode.J)) offset.x -= velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.L)) offset.x += velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.I)) offset.z += velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.K)) offset.z -= velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.U)) offset.y += velocidadControladores * Time.deltaTime;
            if (Input.GetKey(KeyCode.O)) offset.y -= velocidadControladores * Time.deltaTime;
            
            rightController.localPosition = Vector3.Lerp(
                rightController.localPosition,
                posicionInicialDer + offset,
                Time.deltaTime * 5f
            );
            
            // Grip derecho - H
            if (Input.GetKeyDown(KeyCode.H))
                SimularGrip(rightController, true);
            if (Input.GetKeyUp(KeyCode.H))
                SimularGrip(rightController, false);
        }
        
        // Triggers
        if (Input.GetMouseButton(0)) // Click izquierdo = Trigger derecho
            SimularTrigger(rightController, true);
        else
            SimularTrigger(rightController, false);
            
        if (Input.GetMouseButton(1)) // Click derecho = Trigger izquierdo
            SimularTrigger(leftController, true);
        else
            SimularTrigger(leftController, false);
    }

    void SimularGrip(Transform controller, bool pressed)
    {
        if (controller == null) return;
        
        var interactor = controller.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor != null && pressed)
        {
            // Aquí puedes agregar lógica específica para el grip
            Debug.Log($"Grip presionado en {controller.name}");
        }
    }

    void SimularTrigger(Transform controller, bool pressed)
    {
        if (controller == null) return;
        
        var interactor = controller.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
        if (interactor != null && pressed)
        {
            // Aquí puedes agregar lógica específica para el trigger
            Debug.Log($"Trigger presionado en {controller.name}");
        }
    }
}

/*
GUÍA DE CONTROLES:
==================
Movimiento del Rig:
- WASD: Mover en el espacio
- Q/E: Rotar el rig
- Mouse: Rotar la cámara
- ESC: Bloquear/desbloquear cursor

Controlador Izquierdo:
- Numpad 4/6: Izquierda/Derecha
- Numpad 8/5: Adelante/Atrás
- Numpad 7/9: Arriba/Abajo
- G: Grip izquierdo
- Click derecho: Trigger izquierdo

Controlador Derecho:
- J/L: Izquierda/Derecha
- I/K: Adelante/Atrás
- U/O: Arriba/Abajo
- H: Grip derecho
- Click izquierdo: Trigger derecho
*/
