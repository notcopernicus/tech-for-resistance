using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraTransform;

    [Header("Settings")]
    public float walkSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -19.62f;

    private float xRotation = 0f;
    private Vector3 velocity;

    void Start()
    {
        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // MOUSE LOOK
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate camera up/down
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotate player left/right
        transform.Rotate(Vector3.up * mouseX);

        // MOVEMENT
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * walkSpeed * Time.deltaTime);

        // GRAVITY
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}