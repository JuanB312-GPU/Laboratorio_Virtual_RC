using UnityEngine;


public class InteractableTracker : MonoBehaviour
{
    [Header("Thresholds")]
    public float moveThreshold = 0.5f;   // units
    public float rotateThreshold = 90f;  // degrees

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start()
    {
        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(args => {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        });
        grab.selectExited.AddListener(args => CheckChanges());
    }

    void CheckChanges()
    {
        float distance = Vector3.Distance(transform.position, lastPosition);
        float angle = Quaternion.Angle(transform.rotation, lastRotation);

        if (distance >= moveThreshold)
            ToastManager.Instance.ShowToast(gameObject.name + " was moved");

        if (angle >= rotateThreshold)
            ToastManager.Instance.ShowToast(gameObject.name + " was rotated 90°");
    }
}
