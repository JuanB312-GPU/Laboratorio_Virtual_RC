using UnityEngine;
using UnityEngine.UI;
using Unity.XR.CoreUtils;



public class DeskPosition : MonoBehaviour
{
    public Toggle myToggle;
    public Vector3 deskPosition = new Vector3(-10.4f, 0.04f, -10.7f);
    public XROrigin xrOrigin;
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionProvider locomotionProvider;

    void Start()
    {
        myToggle.isOn = false;
    }

    public void OnValueChanged(bool isOn)
    {
        if (isOn)
        {
            xrOrigin.transform.position = deskPosition;
            locomotionProvider.enabled = false;
        }
        else
        {
            locomotionProvider.enabled = true;
        }
    }
}
