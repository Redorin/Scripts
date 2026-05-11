using UnityEngine;

// Attach to an invisible trigger zone at the server room entrance.
// Locks the exit door when player enters.
// CableConnectionPuzzleManager calls Unlock() when puzzle is solved.

public class ServerRoomLockdown : MonoBehaviour
{
    [Header("References")]
    public InteractableDoor exitDoor;
    public KeycardDoor keycardDoor;         // if exit also has keycard door
    public CableConnectionPuzzleManager puzzleManager;

    [Header("Key Spawn")]
    public KeySpawner keySpawner;           // spawns archive key on puzzle complete

    [Header("Dialogue")]
    public string lockdownMessage   = "Emergency lockdown initiated. Complete diagnostics to restore access.";
    public string[] entryDialogue   = {
        "Arzatech server grid — offline.",
        "Identify fault and restore connection."
    };

    private bool hasTriggered = false;

    void Start()
    {
        // Make sure exit door starts locked
        if (exitDoor != null) exitDoor.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        TriggerLockdown();
    }

    void TriggerLockdown()
    {
        // Lock exit
        if (exitDoor != null) exitDoor.enabled = false;
        if (keycardDoor != null) keycardDoor.isUnlocked = false;

        if (AdminDialogue.Instance != null)
        {
            AdminDialogue.Instance.AdminWarning(lockdownMessage);
            foreach (string line in entryDialogue)
                AdminDialogue.Instance.AdminInfo(line);
        }
    }

    // Called by CableConnectionPuzzleManager.SolvePuzzle()
    public void Unlock()
    {
        if (exitDoor != null) exitDoor.enabled = true;
        if (keycardDoor != null) keycardDoor.isUnlocked = true;

        // Spawn archive key
        if (keySpawner != null) keySpawner.SpawnKey();

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo("Lockdown lifted. Exit restored.");
    }
}