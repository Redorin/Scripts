using UnityEngine;

// Attach to the override panel in the maintenance room.
// Becomes usable only after server room puzzle is solved.
// Removes the barricade blocking the 4th floor stairs.

public class MaintenanceOverridePanel : MonoBehaviour
{
    [Header("References")]
    public GameObject barricade;                        // barricade blocking 4th floor stairs
    public CableConnectionPuzzleManager serverPuzzle;   // check if server room is done

    [Header("Dialogue")]
    public string notReadyMessage    = "Override panel — locked. Complete server room diagnostics first.";
    public string activateMessage    = "Maintenance override activated. 4th floor access restored.";
    public string alreadyDoneMessage = "4th floor access already restored.";

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

        // Only usable after server room puzzle is complete
        if (serverPuzzle == null || !serverPuzzle.isSolved)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(notReadyMessage);
            return;
        }

        Activate();
    }

    void Activate()
    {
        isActivated = true;

        // Remove barricade
        if (barricade != null)
            barricade.SetActive(false);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(activateMessage);

        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);
    }
}