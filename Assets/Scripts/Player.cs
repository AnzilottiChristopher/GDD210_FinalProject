using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0f;

    [SerializeField]
    private CharacterController controller;

    private float xRotation = 0f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera;
    public Vector3 velocity;
    public float gravity = -9.81f;

    [Header("Health Settings")]
    [SerializeField] private int health;

    [Header("Crouch Settings")]
    public float crouchHeight = 1.0f;
    public float standingHeight = 2.0f;
    public float crouchSpeed = 2.5f;
    public bool isCrouching = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        health = 3;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleCamera();
        handleGravity();
        handleCrouch();
    }

    private void handleCamera()
    {
        //Mouse Camera Controls
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    private void handleGravity()
    {
        //gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; //small negative value to keep player grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    } 

    private void handleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Adjust speed based on crouch state
        float currentSpeed = isCrouching ? crouchSpeed : speed;

        //Player Movement Controls
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void handleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                // Crouch
                controller.height = crouchHeight;
                controller.center = new Vector3(0, crouchHeight / 2f, 0);
            }
            else
            {
                // Stand up without checking for obstacles
                controller.height = standingHeight;
                controller.center = new Vector3(0, standingHeight / 6f, 0);
            }
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}