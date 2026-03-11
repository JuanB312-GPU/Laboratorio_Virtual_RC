using UnityEngine;

public class InteractableTracker : MonoBehaviour
{
    [Header("Thresholds")]
    public float moveThreshold = 0.5f;
    public float rotateThreshold = 90f;

    private Vector3 lastPosition;
    private Vector3 lastRotationEuler;

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
        float distance = Vector3.Distance(transform.position, lastPosition);

        Vector3 currentEuler = transform.eulerAngles;

        float deltaX = Mathf.Abs(Mathf.DeltaAngle(lastRotationEuler.x, currentEuler.x));
        float deltaY = Mathf.Abs(Mathf.DeltaAngle(lastRotationEuler.y, currentEuler.y));

        float moveScore = distance / moveThreshold;
        float rotXScore = deltaX / rotateThreshold;
        float rotYScore = deltaY / rotateThreshold;

        float maxScore = Mathf.Max(moveScore, rotXScore, rotYScore);

        if (maxScore < 1f)
            return; // nada superó el threshold

        if (maxScore == moveScore)
        {
            ToastManager.Instance.ShowToast("Movimiento");
        }
        else if (maxScore == rotXScore)
        {
            ToastManager.Instance.ShowToast("Rotación en X");
        }
        else
        {
            ToastManager.Instance.ShowToast("Rotación en Y");
        }
    }
}
