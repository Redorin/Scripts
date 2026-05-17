// File: MaintenanceOverridePanel.cs
using UnityEngine;

public class MaintenanceOverridePanel : MonoBehaviour
{
    [Header("References")]
    public GameObject barricade;
    public CableConnectionPuzzleManager serverPuzzle;
    public ArchiveRoomTracker archiveTracker;

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

        if (serverPuzzle == null || !serverPuzzle.isSolved)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(notReadyMessage);
            return;
        }

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

        // Fire objective trigger — completes activate_override, adds reach_4th_floor
        GetComponent<ObjectiveTrigger>()?.TriggerObjective();

        if (barricade != null)
            barricade.SetActive(false);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(activateMessage);

        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);
    }
}