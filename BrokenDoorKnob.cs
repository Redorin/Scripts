using UnityEngine;
using System.Collections;

// Attach to DoorPivot (4).
// Door starts with InteractableDoor DISABLED so player cannot open it manually.
// Player must use Reset Device on DoorKnob to open it.
// After player enters, door closes and locks permanently, then power outage fires.

public class BrokenDoorknob : MonoBehaviour
{
    [Header("References")]
    public GameObject doorObject;               // Drag MaintenanceDoorEntry GameObject here
    public Renderer doorknobRenderer;           // Drag DoorKnob here
    public PowerOutageEvent powerOutage;        // Drag PowerOutageTrigger here

    [Header("Colors")]
    public Color brokenColor = Color.red;
    public Color fixedColor = Color.green;
    public Color rebrokenColor = new Color(0.4f, 0f, 0f);

    [Header("Dialogue")]
    public string[] brokenDialogue = {
        "Door mechanism: corrupted.",
        "Rollback required."
    };
    public string[] fixedDialogue = {
        "Mechanism restored.",
        "Entry permitted."
    };
    public string[] rebrokenDialogue = {
        "Correction limit reached.",
        "Object integrity: unrecoverable."
    };

    [Header("State")]
    public bool isFixed = false;
    public bool isPermanentlyBroken = false;

    private InteractableDoor door;

    void Start()
    {
        // Get InteractableDoor component from the assigned GameObject
        if (doorObject != null)
            door = doorObject.GetComponent<InteractableDoor>();

        // DISABLE the door so player cannot interact with it normally
        if (door != null)
            door.enabled = false;

        // Start knob red
        if (doorknobRenderer != null)
            doorknobRenderer.material.color = brokenColor;

        // Show broken dialogue after short delay
        StartCoroutine(ShowBrokenDialogue());
    }

    IEnumerator ShowBrokenDialogue()
    {
        yield return new WaitForSeconds(1.5f);
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in brokenDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }
    }

    // Called by DoorKnobResettable when player uses Reset Device on knob
    public void OnKnobReset()
    {
        if (isFixed || isPermanentlyBroken) return;
        isFixed = true;

        // Knob turns green
        if (doorknobRenderer != null)
            doorknobRenderer.material.color = fixedColor;

        // Re-enable InteractableDoor so player can now push it open
        if (door != null)
        {
            door.enabled = true;
            door.Interact(); // Auto-open it
        }

        // Admin dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in fixedDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        Debug.Log("Doorknob reset - door opened.");
    }

    // Called by RoomEntryTrigger when player walks inside
    public void OnPlayerEnteredRoom()
    {
        if (isPermanentlyBroken) return;
        isPermanentlyBroken = true;
        StartCoroutine(BreakAgainSequence());
    }

    IEnumerator BreakAgainSequence()
    {
        yield return new WaitForSeconds(0.3f);

        // Close and disable door permanently
        if (door != null)
        {
            if (door.IsOpen()) door.Interact();
            door.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        // Knob turns dark red
        if (doorknobRenderer != null)
            doorknobRenderer.material.color = rebrokenColor;

        // Admin dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in rebrokenDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        yield return new WaitForSeconds(1f);

        // Trigger power outage
        if (powerOutage != null)
            powerOutage.TriggerOutage();

        Debug.Log("Door broke again - power outage triggered.");
    }
}