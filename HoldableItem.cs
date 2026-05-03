using UnityEngine;

public class HoldableItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Item";
    
   [Header("Hold Settings")]
public Vector3 holdPositionOffset = Vector3.zero;
public Vector3 holdRotationOffset = Vector3.zero;
    
    private Rigidbody rb;
    private Collider itemCollider;
    private bool isBeingHeld = false;
    private bool isActive = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        itemCollider = GetComponent<Collider>();
    }
    
    public void PickUp(Transform holdPoint)
    {
        isBeingHeld = true;
        
        // Disable physics
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        // Disable collision with player
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
        
        // Attach to hold point
        transform.SetParent(holdPoint);
        transform.localPosition = holdPositionOffset;
        transform.localRotation = Quaternion.Euler(holdRotationOffset);
        
        Debug.Log("Picked up: " + itemName);
    }
    
    public void Drop()
    {
        isBeingHeld = false;
        isActive = false;
        
        // Detach from player
        transform.SetParent(null);
        
        // Re-enable physics
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        
        // Re-enable collision
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }
        
        // Make visible
        gameObject.SetActive(true);
        
        Debug.Log("Dropped: " + itemName);
    }
    
    public void SetActive(bool active)
    {
        isActive = active;
        gameObject.SetActive(active);
    }
    
    public bool IsBeingHeld()
    {
        return isBeingHeld;
    }
    
    public bool IsActive()
    {
        return isActive;
    }
}