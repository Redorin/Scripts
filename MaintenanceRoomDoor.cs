using UnityEngine;
using System.Collections;

// Attach to DoorPivot (4) - the maintenance room entry door.
// Flow:
// 1. Door starts locked. Handle is red. Lights are ON.
// 2. Player resets handle -> handle turns cyan -> door unlocks.
// 3. Player presses E -> door opens.
// 4. Player walks through RoomEntryTrigger -> door force-closes -> lights OFF.
// 5. Player interacts with FuseBox -> lights ON -> exit door unlocks.

public class MaintenanceRoomDoor : MonoBehaviour
{
    [Header("Door Handle")]
    public Renderer handleRenderer;
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.cyan;

    [Header("Room Lights")]
    public Light[] roomLights;

    [Header("Exit Door")]
    public GameObject exitDoorPivot;

    [Header("Fuse Box")]
    public FuseBoxPuzzle fuseBox;

    [Header("Dialogue")]
    public string[] lockedDoorDialogue = { "Door mechanism: corrupted.", "Reset the handle first." };
    public string[] handleResetDialogue = { "Mechanism restored.", "Entry permitted." };
    public string[] roomEntryDialogue = { "Correction limit reached.", "Object integrity: unrecoverable." };
    public string powerOutageDialogue = "Power failure detected. Find the fuse box.";

    [Header("State")]
    public bool handleIsReset = false;
    public bool playerHasEntered = false;
    public bool isPermanentlyLocked = false;

    private InteractableDoor door;
    private InteractableDoor exitDoor;
    private bool dialogueShown = false;

    void Start()
    {
        door = GetComponent<InteractableDoor>();

        if (exitDoorPivot != null)
            exitDoor = exitDoorPivot.GetComponent<InteractableDoor>();

        // Handle starts red
        if (handleRenderer != null)
            handleRenderer.material.color = lockedColor;

        // Exit door starts locked
        if (exitDoor != null)
            exitDoor.enabled = false;

        // Lights start ON - do NOT touch them here
        // FuseBoxPuzzle must also NOT call SetLights in its Start()
        SetLights(true);
    }

    void Update()
    {
        // Show locked dialogue once after short delay on first approach
        // We use a simple timer instead of coroutine to avoid Start() race condition
    }

    public void TryOpen()
    {
        if (isPermanentlyLocked) return;

        if (!handleIsReset)
        {
            if (!dialogueShown)
            {
                dialogueShown = true;
                if (AdminDialogue.Instance != null)
                {
                    foreach (string line in lockedDoorDialogue)
                        AdminDialogue.Instance.AdminWarning(line);
                }
            }
            else
            {
                // Allow repeated press to show dialogue again
                dialogueShown = false;
            }
            return;
        }

        // Handle reset - toggle door open/close
        if (door != null)
            door.Interact();
    }

    public void OnHandleReset()
    {
        if (handleIsReset) return;
        handleIsReset = true;

        if (handleRenderer != null)
            handleRenderer.material.color = unlockedColor;

        if (AdminDialogue.Instance != null)
        {
            foreach (string line in handleResetDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);

        Debug.Log("Handle reset - door unlocked.");
    }

    public void OnPlayerEntered()
    {
        if (playerHasEntered) return;
        playerHasEntered = true;
        StartCoroutine(RoomEntrySequence());
    }

    IEnumerator RoomEntrySequence()
    {
        yield return new WaitForSeconds(0.2f);

        // Force close door completely - instant snap, no lerp
        if (door != null)
        {
            door.ForceClose();
            door.enabled = false;
        }

        // Lock permanently
        isPermanentlyLocked = true;

        yield return new WaitForSeconds(0.3f);

        // Lights off
        SetLights(false);

        if (AdminDialogue.Instance != null)
        {
            foreach (string line in roomEntryDialogue)
                AdminDialogue.Instance.AdminWarning(line);

            yield return new WaitForSeconds(2f);

            AdminDialogue.Instance.AdminWarning(powerOutageDialogue);
        }

        StartCoroutine(WaitForFuseBox());
    }

    IEnumerator WaitForFuseBox()
    {
        while (fuseBox != null && !fuseBox.isSolved)
            yield return new WaitForSeconds(0.2f);

        SetLights(true);

        if (exitDoor != null)
            exitDoor.enabled = true;

        if (AdminDialogue.Instance != null)
        {
            AdminDialogue.Instance.AdminWarning("Power restored.");
            AdminDialogue.Instance.AdminWarning("Exit door unlocked.");
        }
    }

    public void SetLights(bool on)
    {
        foreach (Light l in roomLights)
            if (l != null) l.enabled = on;
    }
}