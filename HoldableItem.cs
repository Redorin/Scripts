// File: HoldableItem.cs
using UnityEngine;

public class HoldableItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Item";

    [Header("Hold Settings")]
    public Vector3 holdPositionOffset = Vector3.zero;
    public Vector3 holdRotationOffset = Vector3.zero;

    [Header("Objective Counter (Optional)")]
    [Tooltip("If set, increments this objective counter when picked up")]
    public string pickupCounterObjectiveID = "";

    private Rigidbody rb;
    private Collider itemCollider;
    private bool isBeingHeld = false;
    private bool isActive = false;
    private Vector3 originalLocalScale;
    private Transform originalParent;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
        originalLocalScale = transform.localScale;
        originalParent = transform.parent;
    }

    public void PickUp(Transform holdPoint)
    {
        isBeingHeld = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (itemCollider != null)
            itemCollider.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = holdPositionOffset;
        transform.localRotation = Quaternion.Euler(holdRotationOffset);
        transform.localScale = originalLocalScale;

        // Increment objective counter if assigned
        if (!string.IsNullOrEmpty(pickupCounterObjectiveID))
            ObjectiveManager.Instance?.IncrementCounter(pickupCounterObjectiveID);

        Debug.Log("Picked up: " + itemName);
    }

    public void Drop()
    {
        isBeingHeld = false;
        isActive = false;

        transform.SetParent(null);
        transform.localScale = originalLocalScale;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (itemCollider != null)
            itemCollider.enabled = true;

        gameObject.SetActive(true);

        Debug.Log("Dropped: " + itemName);
    }

    public void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
    }

    public bool IsBeingHeld() => isBeingHeld;
    public bool IsActive()    => isActive;
}