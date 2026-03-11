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
        }
    }

}
