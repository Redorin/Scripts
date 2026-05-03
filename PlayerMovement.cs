using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float gravity = -20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Stair Descent")]
    public float groundedGravity = -2f;

    [Header("Head Bob (Optional)")]
    public bool useHeadBob = false;
    public float bobFrequency = 2f;
    public float bobAmplitude = 0.05f;
    public Transform cameraTransform;

    private CharacterController controller;
    private float verticalVelocity = 0f;
    private bool isGrounded;
    private Vector2 moveInput;
    private Vector3 lastPosition;
    private bool isMoving;
    private float bobTimer = 0f;
    private float defaultCameraY;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position;

        if (cameraTransform != null)
            defaultCameraY = cameraTransform.localPosition.y;
    }

    void Update()
    {
        // ✅ Use built-in grounded check (more stable)
        isGrounded = controller.isGrounded;

        // Input
        moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

            if (moveInput.magnitude > 1f)
                moveInput.Normalize();
        }

        // Horizontal movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Vertical movement
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = groundedGravity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // ✅ SINGLE Move call (fixes teleport bug)
        Vector3 finalMove = move * walkSpeed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        // Track movement for head bob
        isMoving = Vector3.Distance(transform.position, lastPosition) > 0.001f && isGrounded;
        lastPosition = transform.position;

        // Head bob
        if (useHeadBob && cameraTransform != null)
        {
            if (isMoving)
            {
                bobTimer += Time.deltaTime * bobFrequency;
                float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
                Vector3 camPos = cameraTransform.localPosition;
                camPos.y = defaultCameraY + bobOffset;
                cameraTransform.localPosition = camPos;
            }
            else
            {
                bobTimer = 0f;
                Vector3 camPos = cameraTransform.localPosition;
                camPos.y = Mathf.Lerp(camPos.y, defaultCameraY, Time.deltaTime * 5f);
                cameraTransform.localPosition = camPos;
            }
        }
    }

    public bool IsMoving() => isMoving;
    public bool IsGrounded() => isGrounded;
}