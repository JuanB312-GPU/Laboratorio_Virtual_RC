using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    private PlayerControls controls;
    private float horizontal;
    private float vertical;
    public float jumpForce = 5f;
    public Rigidbody component_rb;

    void Start()
    {
        component_rb = GetComponent<Rigidbody>();
    }

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Horizontal.performed += ctx => horizontal = ctx.ReadValue<float>();
        controls.Player.Horizontal.canceled += ctx => horizontal = 0;
        controls.Player.Vertical.performed += ctx => vertical = ctx.ReadValue<float>();
        controls.Player.Vertical.canceled += ctx => vertical = 0;

        controls.Player.Jump.performed += ctx => Jump();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Update()
    {
        Vector3 movement = new Vector3(horizontal, 0, vertical);
        transform.Translate(movement * speed * Time.deltaTime);
    }

    public void Jump()
    {
        if (Mathf.Abs(component_rb.linearVelocity.y) < 0.01f) // Para evitar salto doble
        {
            component_rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}