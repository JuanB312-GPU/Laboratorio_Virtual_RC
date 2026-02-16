using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapToRack : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private RackSlot currentSlot;
    public Transform snapPoint;


    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void SnapIntoSlot()
    {
    // Buscamos el SnapPoint dentro del Slot

    if (snapPoint == null)
    {
        Debug.LogWarning("No SnapPoint found inside slot.");
        return;
    }

    // Hacemos hijo del SnapPoint
    transform.SetParent(snapPoint);

    // Reseteamos posición y rotación LOCAL
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;

    currentSlot.isOccupied = true;

    // Bloqueamos física
    Rigidbody rb = GetComponent<Rigidbody>();
    rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        RackSlot slot = other.GetComponent<RackSlot>();

        if (slot != null && !slot.isOccupied)
        {
            currentSlot = slot;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RackSlot slot = other.GetComponent<RackSlot>();

        if (slot != null && slot == currentSlot)
        {
            currentSlot = null;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (currentSlot != null && !currentSlot.isOccupied)
        {
            SnapIntoSlot();
        }
    }

}
