using UnityEngine;

public class TypingArea : MonoBehaviour
{
    public GameObject leftTypingHand;
    public GameObject rightTypingHand;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("LeftHand"))
        {
            leftTypingHand.SetActive(true);
        }
        else if (other.transform.root.CompareTag("RightHand"))
        {
            rightTypingHand.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("LeftHand"))
        {
            leftTypingHand.SetActive(false);
        }
        else if (other.transform.root.CompareTag("RightHand"))
        {
            rightTypingHand.SetActive(false);
        }
    }
}
