using UnityEngine;
using System.Collections;

// Attach to the DoorKnob child object.
// This single script handles the entire maintenance room door sequence:
// 1. Knob starts red. Door is locked.
// 2. Player uses Reset Device on knob. Knob turns cyan. Door unlocks.
// 3. Player enters room. Lights go off. Door closes and locks again permanently.
// 4. Player interacts with FuseBox. Lights come back on. Exit door unlocks.

public class DoorKnob : MonoBehaviour
{
    [Header("Door Reference")]
    public GameObject doorPivot;            // Drag DoorPivot (4) here

    [Header("Room Lights")]
    public Light[] roomLights;              // All lights inside the maintenance room

    [Header("Exit Door")]
    public GameObject exitDoorPivot;        // Drag DoorPivot (5) here

    [Header("Fuse Box")]
    public FuseBoxPuzzle fuseBox;           // Drag FuseBox here

    [Header("Colors")]
    public Color brokenColor = Color.red;
    public Color fixedColor = Color.cyan;

    [Header("Dialogue")]
    public string[] lockedDialogue = { "Door mechanism: corrupted.", "Rollback required." };
    public string[] unlockedDialogue = { "Mechanism restored.", "Entry permitted." };
    public string[] roomEntryDialogue = { "Correction limit reached.", "Object integrity: unrecoverable." };

    [Header("State")]
    public bool isUnlocked = false;
    public bool playerHasEntered = false;

    private Renderer knobRenderer;
    private InteractableDoor entryDoor;
    private InteractableDoor exitDoor;

    void Start()
    {
        knobRenderer = GetComponent<Renderer>();

        // Get door components
        if (doorPivot != null)
            entryDoor = doorPivot.GetComponent<InteractableDoor>();

        if (exitDoorPivot != null)
            exitDoor = exitDoorPivot.GetComponent<InteractableDoor>();

        // Start knob red
        if (knobRenderer != null)
            knobRenderer.material.color = brokenColor;

        // Lock both doors at start
        if (entryDoor != null) entryDoor.enabled = false;
        if (exitDoor != null) exitDoor.enabled = false;

        // Lights start ON (power outage happens after entry)
        SetLights(true);

        // Show locked dialogue after short delay
        StartCoroutine(ShowDialogueDelayed(lockedDialogue, 1.5f));
    }

    // Called by ResettableObject when player uses Reset Device on this knob
    // Since we only want color change (no position change), set ResettableObject:
    // resetPosition = false, resetRotation = false, resetScale = false
    // The actual unlock logic happens here via DoorKnobBridge
    public void OnReset()
    {
        if (isUnlocked) return;
        isUnlocked = true;

        // Turn knob cyan
        if (knobRenderer != null)
            knobRenderer.material.color = fixedColor;

        // Unlock entry door
        if (entryDoor != null)
            entryDoor.enabled = true;

        // Admin dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in unlockedDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        // Small instability increase
        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);

        Debug.Log("Knob reset - entry door unlocked.");
    }

    // Called by RoomEntryTrigger when player walks through
    public void OnPlayerEntered()
    {
        if (playerHasEntered) return;
        playerHasEntered = true;
        StartCoroutine(RoomEntrySequence());
    }

    IEnumerator RoomEntrySequence()
    {
        yield return new WaitForSeconds(0.3f);

        // Close and lock entry door permanently
        if (entryDoor != null)
        {
            if (entryDoor.IsOpen()) entryDoor.Interact();
            entryDoor.enabled = false;
        }

        yield return new WaitForSeconds(0.4f);

        // Lights go off
        SetLights(false);

        // Admin dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in roomEntryDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        // Now wait for fuse box to be solved
        StartCoroutine(WaitForFuseBox());
    }

    IEnumerator WaitForFuseBox()
    {
        // Poll until fusebox is solved
        while (fuseBox != null && !fuseBox.isSolved)
            yield return new WaitForSeconds(0.2f);

        // Lights back on
        SetLights(true);

        // Unlock exit door
        if (exitDoor != null)
            exitDoor.enabled = true;

        Debug.Log("Fuse box solved - lights on, exit unlocked.");
    }

    void SetLights(bool on)
    {
        foreach (Light l in roomLights)
            if (l != null) l.enabled = on;
    }

    IEnumerator ShowDialogueDelayed(string[] lines, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in lines)
                AdminDialogue.Instance.AdminWarning(line);
        }
    }
}