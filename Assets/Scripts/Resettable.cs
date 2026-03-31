using UnityEngine;

public class Resettable : MonoBehaviour
{
    // Estado inicial guardado
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool initialActive;
    private InteractableTracker tracker;

    // Para objetos con Rigidbody
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tracker = GetComponent<InteractableTracker>(); // mismo objeto
        SaveState();
    }

    public void SaveState()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale    = transform.localScale;
        initialActive   = gameObject.activeSelf;
    }

    public void ResetState()
    {
        // Restaurar transform
        transform.position   = initialPosition;
        transform.rotation   = initialRotation;
        transform.localScale = initialScale;
        gameObject.SetActive(initialActive);

        // Detener física si tiene Rigidbody
        if (rb != null)
        {
            rb.linearVelocity        = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (tracker != null){
            tracker.ResetFlags();
        }
    }
}
