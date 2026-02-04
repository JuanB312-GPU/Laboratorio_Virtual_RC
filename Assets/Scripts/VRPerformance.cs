using UnityEngine;
using UnityEngine.XR;

public class VRPerformance : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 72;
        XRSettings.eyeTextureResolutionScale = 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
