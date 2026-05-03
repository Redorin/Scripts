using UnityEngine;

// Attach to the DoorKnob object alongside ResettableObject.
// Detects when ResettableObject gets used and notifies MaintenanceRoomDoor.
// ResettableObject settings on DoorKnob:
//   - Reset Position = FALSE
//   - Reset Rotation = FALSE  
//   - Reset Physics = FALSE
//   - Can Be Reset = TRUE
//   - Max Reset Uses = 1

public class DoorHandleBridge : MonoBehaviour
{
    public MaintenanceRoomDoor doorManager;  // Drag DoorPivot (4) here

    private ResettableObject resettable;
    private int lastUses = 0;

    void Start()
    {
        resettable = GetComponent<ResettableObject>();
    }

    void Update()
    {
        if (resettable == null || doorManager == null) return;

        int usedCount = resettable.maxResetUses - resettable.GetRemainingUses();
        if (usedCount > lastUses)
        {
            lastUses = usedCount;
            doorManager.OnHandleReset();
        }
    }
}