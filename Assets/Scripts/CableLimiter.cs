using UnityEngine;


public class CableLimiter : MonoBehaviour
{
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabA;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabB;

    public float maxDistance = 1.5f;

    private Rigidbody rbA;
    private Rigidbody rbB;

    void Start()
    {
        rbA = grabA.GetComponent<Rigidbody>();
        rbB = grabB.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        float distance = Vector3.Distance(grabA.transform.position, grabB.transform.position);

        if (distance > maxDistance)
        {
            Vector3 direction = (grabB.transform.position - grabA.transform.position).normalized;

            if (grabA.isSelected)
            {
                grabB.transform.position = grabA.transform.position + direction * maxDistance;

                rbB.linearVelocity = Vector3.zero;
                rbB.angularVelocity = Vector3.zero;
            }
            else if (grabB.isSelected)
            {
                grabA.transform.position = grabB.transform.position - direction * maxDistance;

                rbA.linearVelocity = Vector3.zero;
                rbA.angularVelocity = Vector3.zero;
            }
        }
    }
}