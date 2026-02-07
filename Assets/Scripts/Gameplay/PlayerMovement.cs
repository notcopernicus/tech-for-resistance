using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraTransform; // Drag your Main Camera here in the Inspector

    [Header("Settings")]
    public float walkSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravity = -19.62f; 

    private float xRotation = 0f;
    private Vector3 velocity;

    void Start()
    {
        // This locks the mouse to the center of the screen so it doesn't fly off
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. MOUSE LOOK
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevents flipping upside down

        // Rotate the camera up and down
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotate the player body left and right
        transform.Rotate(Vector3.up * mouseX);

        // 2. MOVEMENT
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate direction relative to where the player faces
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * walkSpeed * Time.deltaTime);

        // 3. GRAVITY & GROUNDING
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}