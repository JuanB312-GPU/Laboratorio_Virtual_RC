using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private Rigidbody rb;
    private float jumpForce = 250f, force = 50f;
    private Vector2 input;
    private PlayerInput playerInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        input = playerInput.actions["Move"].ReadValue<Vector2>();
        Debug.Log("Move: " + input);
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(input.x, 0.0f, input.y) * force);
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if(context.performed)
        {
             rb.AddForce(Vector3.up * jumpForce);
       Debug.Log("Jumped");
       Debug.Log(context.phase);
        }

    }

}
