using UnityEngine;

public class FirstTestController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleStepMarker(string markerName)
    {
        InitialTestManager.Instance.ShowToast(markerName);
        // Aquí va tu acción: sumar puntos, abrir puerta, etc.
    }
}
