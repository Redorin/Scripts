using UnityEngine;

// Place just inside the Maintenance Room entrance.
// Box Collider - Is Trigger checked.
// When player walks through, notifies MaintenanceRoomDoor.

public class RoomEntryTrigger : MonoBehaviour
{
    public MaintenanceRoomDoor doorManager;     // Drag DoorPivot (4) here

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (doorManager != null)
            doorManager.OnPlayerEntered();

        Debug.Log("Player entered maintenance room.");
    }
}