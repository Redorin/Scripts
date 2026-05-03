using UnityEngine;

// Attach to DoorPivot (5) - the MaintenanceDoorExit.
// Door starts with InteractableDoor DISABLED.
// Automatically unlocks when FuseBoxPuzzle is solved.

public class LockedUntilFusebox : MonoBehaviour
{
    [Header("References")]
    public FuseBoxPuzzle fuseBox;

    [Header("Dialogue")]
    public string lockedDialogue = "Exit sealed. Restore power first.";

    [Header("State")]
    public bool isUnlocked = false;     // PUBLIC so PlayerInteraction can read it

    private InteractableDoor door;

    void Start()
    {
        door = GetComponent<InteractableDoor>();

        if (door != null)
            door.enabled = false;
    }

    void Update()
    {
        if (!isUnlocked && fuseBox != null && fuseBox.isSolved)
            Unlock();
    }

    public void Unlock()
    {
        if (isUnlocked) return;
        isUnlocked = true;

        if (door != null)
            door.enabled = true;

        Debug.Log("Exit door unlocked.");
    }

    public void TryInteractWhileLocked()
    {
        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminWarning(lockedDialogue);
    }
}