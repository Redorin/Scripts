using UnityEngine;

// Attach to the override panel in the maintenance room.
// Requires server room puzzle complete AND archive room visited.
// Removes the barricade blocking the 4th floor stairs.

public class MaintenanceOverridePanel : MonoBehaviour
{
    [Header("References")]
    public GameObject barricade;
    public CableConnectionPuzzleManager serverPuzzle;
    public ArchiveRoomTracker archiveTracker;       // player must visit archive first

    [Header("Dialogue")]
    public string notReadyMessage     = "Override panel — locked. Complete server room diagnostics first.";
    public string archiveFirstMessage = "Override panel — locked. Check the archive room first.";
    public string activateMessage     = "Maintenance override activated. 4th floor access restored.";
    public string alreadyDoneMessage  = "4th floor access already restored.";

    [Header("State")]
    public bool isActivated = false;

    public void Interact()
    {
        if (isActivated)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(alreadyDoneMessage);
            return;
        }

        // Step 1 — server room must be done first
        if (serverPuzzle == null || !serverPuzzle.isSolved)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(notReadyMessage);
            return;
        }

        // Step 2 — player must have visited archive room
        if (archiveTracker == null || !archiveTracker.hasVisited)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(archiveFirstMessage);
            return;
        }

        Activate();
    }

    void Activate()
    {
        isActivated = true;
        Chapter1Objectives.Instance?.Complete_OverridePanel();

        if (barricade != null)
            barricade.SetActive(false);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(activateMessage);

        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);
    }
}