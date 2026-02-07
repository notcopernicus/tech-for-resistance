using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraTransform;

    [Header("Settings")]
    public float walkSpeed = 5f;
    public float sensitivity = 2f;
    public float gravity = -9.81f;

    private float xRotation = 0f;
    private Vector3 velocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. LOOK LOGIC (Mouse)
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Tilt the camera up/down
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Rotate the whole player left/right
        transform.Rotate(Vector3.up * mouseX);

        // 2. MOVE LOGIC (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Move relative to where the player faces
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * walkSpeed * Time.deltaTime);

        // 3. GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Reset velocity when grounded
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }
    }
}