using UnityEngine;
using UnityEngine.InputSystem;

public class ResetItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Reset Device";

    [Header("Hold Settings")]
    public Vector3 holdPositionOffset = new Vector3(0, -0.3f, 0.5f);
    public Vector3 holdRotationOffset = Vector3.zero;

    [Header("Reset Settings")]
    public float resetRange = 10f;

    private Rigidbody rb;
    private Collider itemCollider;
    private bool isBeingHeld = false;
    private bool isActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (isBeingHeld && isActive)
        {
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                UseItem();
        }
    }

    public void PickUp(Transform holdPoint)
    {
        isBeingHeld = true;

        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }
        if (itemCollider != null) itemCollider.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = holdPositionOffset;
        transform.localRotation = Quaternion.Euler(holdRotationOffset);

        Debug.Log("Picked up: " + itemName + " (Press R to use)");
    }

    public void Drop()
    {
        isBeingHeld = false;
        isActive = false;

        transform.SetParent(null);

        if (rb != null) { rb.isKinematic = false; rb.useGravity = true; }
        if (itemCollider != null) itemCollider.enabled = true;

        gameObject.SetActive(true);
        Debug.Log("Dropped: " + itemName);
    }

    public void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
    }

    void UseItem()
    {
        Camera cam = Camera.main;
        if (cam == null) { Debug.LogWarning("No main camera found!"); return; }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, resetRange))
        {
            // Priority 1 - Debris piece (resets whole group at once)
            DebrisPiece debrisPiece = hit.collider.GetComponent<DebrisPiece>();
            if (debrisPiece != null)
            {
                debrisPiece.NotifyGroup();
                return;
                // Instability and dialogue handled by DebrisGroup
            }

            // Priority 2 - Chapter 3 student (block reset)
            StudentObject student = hit.collider.GetComponent<StudentObject>();
            if (student != null)
            {
                student.AttemptReset();
                return;
            }

            // Priority 3 - Chapter 5 final choice
            FinalChoiceHandler finalChoice = hit.collider.GetComponent<FinalChoiceHandler>();
            if (finalChoice != null)
            {
                finalChoice.OnReset();
                if (InstabilityManager.Instance != null)
                    InstabilityManager.Instance.IncreaseInstability(5f);
                return;
            }

            // Priority 4 - Normal resettable object
            ResettableObject resettable = hit.collider.GetComponent<ResettableObject>();
            if (resettable != null)
            {
                bool success = resettable.Reset();
                if (success)
                {
                    Debug.Log("Successfully reset: " + resettable.objectName);
                    if (InstabilityManager.Instance != null)
                        InstabilityManager.Instance.IncreaseInstability(5f);
                }
                else
                {
                    Debug.Log("Failed to reset: " + resettable.objectName);
                }
            }
            else
            {
                Debug.Log("This object cannot be reset.");
                if (AdminDialogue.Instance != null)
                    AdminDialogue.Instance.AdminWarning("Target object incompatible with rollback protocol.");
            }
        }
        else
        {
            Debug.Log("Nothing in range to reset.");
        }
    }

    public bool IsBeingHeld() => isBeingHeld;
    public bool IsActive() => isActive;
}