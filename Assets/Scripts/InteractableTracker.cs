using UnityEngine;

public class InteractableTracker : MonoBehaviour
{
    [Header("Thresholds")]
    public float moveThreshold = 0.5f;
    public float rotateThreshold = 90f;

    private Vector3 lastPosition;
    private Vector3 lastRotationEuler;

    // Flags para bloquear eventos ya completados
    private bool movementDone = false;
    private bool rotateXDone  = false;
    private bool rotateYDone  = false;

    void Start()
    {
        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grab.selectEntered.AddListener(args => {
            lastPosition = transform.position;
            lastRotationEuler = transform.eulerAngles;
        });

        grab.selectExited.AddListener(args => CheckChanges());
    }

    void CheckChanges()
    {
        float distance     = Vector3.Distance(transform.position, lastPosition);
        Vector3 currentEuler = transform.eulerAngles;
        float deltaX       = Mathf.Abs(Mathf.DeltaAngle(lastRotationEuler.x, currentEuler.x));
        float deltaY       = Mathf.Abs(Mathf.DeltaAngle(lastRotationEuler.y, currentEuler.y));

        // Solo calcular scores de eventos que aún no se han completado
        float moveScore = movementDone ? -1f : distance / moveThreshold;
        float rotXScore = rotateXDone  ? -1f : deltaX   / rotateThreshold;
        float rotYScore = rotateYDone  ? -1f : deltaY   / rotateThreshold;

        float maxScore = Mathf.Max(moveScore, rotXScore, rotYScore);

        if (maxScore < 1f)
            return; // nada superó el threshold o todo ya estaba completado

        if (maxScore == moveScore)
        {
            movementDone = true;
            ToastManager.Instance.ShowToast("Movimiento");
        }
        else if (maxScore == rotXScore)
        {
            rotateXDone = true;
            ToastManager.Instance.ShowToast("Giro en X");
        }
        else
        {
            rotateYDone = true;
            ToastManager.Instance.ShowToast("Giro en Y");
        }

    }

    // Llamar esto desde resetTest() para reiniciar los flags
    public void ResetFlags()
    {
        movementDone = false;
        rotateXDone  = false;
        rotateYDone  = false;
    }
}
