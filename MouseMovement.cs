using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 0.15f;  // Lowered — no longer multiplied by Time.deltaTime

    public Transform mainCamera;

    float xRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Don't move camera when cursor is unlocked (paused)
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // ReadUnprocessedValue() bypasses OS mouse acceleration and smoothing
        Vector2 mouseDelta = Mouse.current.delta.ReadUnprocessedValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        if (mainCamera != null)
            mainCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    public void SetXRotation(float value)
    {
        xRotation = value;
    }

    public float GetXRotation()
    {
        return xRotation;
    }

    public void SetBodyYRotation(float yAngle)
    {
        Vector3 euler = transform.eulerAngles;
        euler.y = yAngle;
        transform.eulerAngles = euler;
    }
}