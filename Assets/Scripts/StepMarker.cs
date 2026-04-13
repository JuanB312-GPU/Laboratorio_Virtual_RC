using UnityEngine;

public class StepMarker : MonoBehaviour
{
    string markerName;

    public void Start()
    {
        markerName = gameObject.name;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponentInParent<FirstTestController>().HandleStepMarker(markerName);
            gameObject.SetActive(false); // Desactiva el marcador para que no se active de nuevo
        }
    }

}
