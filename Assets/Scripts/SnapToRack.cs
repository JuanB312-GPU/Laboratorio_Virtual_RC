using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapToRack : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private List<RackSlot> nearbySlots = new List<RackSlot>();
    private RackSlot snappedSlot;
    private Quaternion originalRotation;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        originalRotation = transform.rotation;

        grabInteractable.selectExited.AddListener(OnRelease);
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnTriggerEnter(Collider other)
    {
        RackSlot slot = other.GetComponent<RackSlot>();
        if (slot != null && !slot.isOccupied)
        {
            if (!nearbySlots.Contains(slot))
            {
                nearbySlots.Add(slot);
                slot.Highlight(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RackSlot slot = other.GetComponent<RackSlot>();
        if (slot != null && nearbySlots.Contains(slot))
        {
            slot.Highlight(false);
            nearbySlots.Remove(slot);
        }
    }

    private RackSlot GetClosestSlot()
    {
        RackSlot closest = null;
        float minDist = float.MaxValue;

        foreach (var slot in nearbySlots)
        {
            Collider slotCollider = slot.GetComponent<Collider>();
            if (slotCollider == null) continue;

            float dist = Vector3.Distance(transform.position, slotCollider.bounds.center);
            if (dist < minDist)
            {
                minDist = dist;
                closest = slot;
            }
        }

        return closest;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (isBeingGrabbed)
        {
            isBeingGrabbed = false;
            RackSlot closestSlot = GetClosestSlot();
            if (closestSlot != null && !closestSlot.isOccupied)
            {
                SnapIntoSlot(closestSlot);
                return;
            }
            // Si no encaja en ninguna slot, deja físicas libres para que siga cayendo/volviendo
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private bool isBeingGrabbed = false;

    private void OnGrab(SelectEnterEventArgs args)
    {
        isBeingGrabbed = true;

        if (snappedSlot != null)
        {
            snappedSlot.isOccupied = false;
            snappedSlot.Highlight(false);
            snappedSlot = null;
        }

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
    }

    private void SnapIntoSlot(RackSlot slot)
    {
        if (slot == null || slot.isOccupied) return;

        transform.SetParent(slot.transform);

        Collider slotCollider = slot.GetComponent<Collider>();
        if (slotCollider != null)
        {
            transform.position = slotCollider.bounds.center;
        }
        else
        {
            transform.position = slot.transform.position;
        }

        transform.rotation = originalRotation;

        slot.isOccupied = true;
        slot.Highlight(true);
        snappedSlot = slot;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}