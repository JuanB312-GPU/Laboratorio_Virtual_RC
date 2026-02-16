using UnityEngine;
  

public class RackSlot : MonoBehaviour
{
    public bool isOccupied = false;
    public Renderer slotRenderer;
    public Material normalMaterial;
    public Material highlightMaterial;

    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied) return;

        // Solo resaltar si el objeto que entra es interactuable
        if (other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>())
        {
            Highlight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isOccupied) return;

        if (other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>())
        {
            Highlight(false);
        }
    }

    public void Highlight(bool state)
    {
        slotRenderer.material = state ? highlightMaterial : normalMaterial;
    }

    void Awake()
    {
        slotRenderer = GetComponentInParent<Renderer>();
        normalMaterial = slotRenderer.material;
    }

}
