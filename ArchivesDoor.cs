using UnityEngine;

// Attach to the Archives door DoorPivot.
// Door starts locked. Player must have picked up the KeyItem to open it.
// When player interacts while having the key, door opens and key is consumed.

public class ArchivesDoor : MonoBehaviour
{
    [Header("Dialogue")]
    public string lockedDialogue = "Door sealed. An access key is required.";
    public string[] unlockDialogue = {
        "Key accepted.",
        "Archives unlocked."
    };

    [Header("State")]
    public bool isUnlocked = false;

    private InteractableDoor door;

    void Start()
    {
        door = GetComponent<InteractableDoor>();

        // Lock door at start
        if (door != null)
            door.enabled = false;
    }

    // Called by PlayerInteraction when player presses E on this door
    public void TryOpen()
    {
        if (isUnlocked)
        {
            // Already unlocked, just interact normally
            if (door != null) door.Interact();
            return;
        }

        // Check if player has the key
        if (KeyItem.Instance != null && KeyItem.Instance.isPickedUp)
        {
            Unlock();
        }
        else
        {
            // No key - play locked dialogue
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(lockedDialogue);
        }
    }

    void Unlock()
    {
        isUnlocked = true;

        // Enable and open door
        if (door != null)
        {
            door.enabled = true;
            door.Interact();
        }

        // Admin dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in unlockDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        // Increase instability
        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);

        Debug.Log("Archives door unlocked.");
    }
}