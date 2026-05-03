using UnityEngine;
using UnityEngine.InputSystem;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform mainCamera;

    float xRotation = 0f;

    public float topClamp = -90f;
    public float bottomClamp = 90f;

    private Vector2 mouseDelta;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

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