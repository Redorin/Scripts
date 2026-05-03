using UnityEngine;

public class DoorWithTrap : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    
    [Header("Trap Settings")]
    public GameObject objectToFall; // Object 1 - the heavy falling object
    public GameObject resetItem; // Object 2 - the reset device
    public float delayBeforeFall = 0.5f;
    
    private bool isOpen = false;
    private bool trapTriggered = false;
    private Quaternion targetRotation;
    private Quaternion closedRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        targetRotation = closedRotation;
        
        // Freeze the falling object initially
        if (objectToFall != null)
        {
            Rigidbody rb = objectToFall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        
        // Freeze the reset item initially
        if (resetItem != null)
        {
            Rigidbody rb = resetItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    public void Interact()
    {
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        targetRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        Debug.Log("Trap door opening!");
        
        if (!trapTriggered)
        {
            trapTriggered = true;
            Invoke("DropObjects", delayBeforeFall);
        }
    }

    void CloseDoor()
    {
        isOpen = false;
        targetRotation = closedRotation;
        Debug.Log("Trap door closing!");
    }
    
    void DropObjects()
    {
        // Drop Object 1 (heavy object)
        if (objectToFall != null)
        {
            Rigidbody rb = objectToFall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                Debug.Log("Object 1 falling!");
            }
        }
        
        // Drop Object 2 (reset item) - with a slight delay
        if (resetItem != null)
        {
            Invoke("DropResetItem", 0.2f); // Falls 0.2 seconds after Object 1
        }
    }
    
    void DropResetItem()
    {
        if (resetItem != null)
        {
            Rigidbody rb = resetItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                Debug.Log("Reset item falling!");
            }
        }
    }
}