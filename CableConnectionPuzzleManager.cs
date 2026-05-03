using UnityEngine;

// One of these in the scene. Drag all sockets into it.
// Checks when all sockets are filled and solves the puzzle.

public class CableConnectionPuzzleManager : MonoBehaviour
{
    [Header("Sockets")]
    public CableSocket[] sockets;

    [Header("On Complete")]
    public GameObject doorToUnlock;
    public bool isSolved = false;

    [Header("Dialogue")]
    public string[] solveDialogue = {
        "Power conduit restored.",
        "Server grid online."
    };

    public void CheckAllConnected()
    {
        if (isSolved) return;

        foreach (CableSocket socket in sockets)
        {
            if (!socket.isFilled) return;
        }

        SolvePuzzle();
    }

    void SolvePuzzle()
    {
        isSolved = true;

        if (doorToUnlock != null)
        {
            InteractableDoor door = doorToUnlock.GetComponent<InteractableDoor>();
            if (door != null) door.Interact();
        }

        if (AdminDialogue.Instance != null)
        {
            foreach (string line in solveDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);

        Debug.Log("Cable puzzle solved!");
    }
}