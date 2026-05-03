using UnityEngine;

// Attach to the BookPuzzleManager empty GameObject inside Archives.
// Correct order: Control (1985), Observation (1992), Correction (1998)
// On solve: removes Fourth Floor blockers, plays admin dialogue.

public class BookArrangementPuzzle : MonoBehaviour
{
    [Header("Book Slots")]
    public BookSlot[] slots;                // Length 3

    [Header("Correct Order")]
    public string[] correctOrder = { "Control", "Observation", "Correction" };

    [Header("On Solve - Remove Blockers")]
    public GameObject[] blockersToRemove;   // Objects blocking staircase/Fourth Floor access

    [Header("On Solve - Dialogue")]
    public string[] solveDialogue = {
        "Unauthorized access logged.",
        "Cross-referencing archive data.",
        "New sector unlocked: Fourth Floor.",
        "Proceed to elevator access point."
    };

    [Header("State")]
    public bool isSolved = false;

    public void CheckOrder()
    {
        if (isSolved) return;
        if (slots.Length != correctOrder.Length) return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetCurrentBook() != correctOrder[i])
                return;
        }

        SolvePuzzle();
    }

    void SolvePuzzle()
    {
        isSolved = true;

        // Remove all blockers
        foreach (GameObject blocker in blockersToRemove)
        {
            if (blocker != null)
                blocker.SetActive(false);
        }

        // Admin dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in solveDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        // Instability increase
        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);

        Debug.Log("Archive puzzle solved - Fourth Floor access granted.");
    }
}