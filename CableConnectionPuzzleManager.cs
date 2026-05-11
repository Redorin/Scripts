using UnityEngine;

public class CableConnectionPuzzleManager : MonoBehaviour
{
    [Header("Cables")]
    public CableConnectionPuzzle[] cables;  // All cables in the puzzle

    [Header("Server Boot Order")]
    public ServerTerminal[] servers;        // Drag in order: servers[0] = first to boot
    public bool isSolved = false;

    [Header("Lockdown")]
    public ServerRoomLockdown lockdown;     // assign in Inspector

    [Header("On Complete")]
    public GameObject doorToUnlock;

    [Header("Dialogue")]
    public string[] cablesSolvedDialogue = {
        "Cable connections restored.",
        "Activate servers in correct sequence."
    };
    public string[] solveDialogue = {
        "Server grid online.",
        "Access to archive granted."
    };

    private bool cablesComplete = false;
    private int currentBootStep = 0;

    // Called by each CableConnectionPuzzle when it connects
    public void OnCableConnected()
    {
        if (cablesComplete) return;

        // Check if all cables are connected
        foreach (CableConnectionPuzzle cable in cables)
            if (!cable.isConnected) return;

        cablesComplete = true;

        // Enable server terminals
        foreach (ServerTerminal server in servers)
            if (server != null) server.SetInteractable(true);

        if (AdminDialogue.Instance != null)
            foreach (string line in cablesSolvedDialogue)
                AdminDialogue.Instance.AdminInfo(line);
    }

    public void OnServerBooted(ServerTerminal server)
    {
        if (isSolved) return;
        if (!cablesComplete) return;

        // Check if this is the correct next server
        if (servers[currentBootStep] == server)
        {
            currentBootStep++;

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(
                    "Server " + currentBootStep + " online.");

            if (currentBootStep >= servers.Length)
                SolvePuzzle();
        }
        else
        {
            // Wrong order — reset boot sequence and increase instability
            currentBootStep = 0;
            foreach (ServerTerminal s in servers)
                if (s != null) s.ResetBoot();

            if (InstabilityManager.Instance != null)
                InstabilityManager.Instance.IncreaseInstability(10f);

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(
                    "Boot sequence error. Instability increased.");
        }
    }

    void SolvePuzzle()
    {
        isSolved = true;

        // Lift server room lockdown and spawn archive key
        if (lockdown != null)
            lockdown.Unlock();

        if (doorToUnlock != null)
        {
            InteractableDoor door = doorToUnlock.GetComponent<InteractableDoor>();
            if (door != null) door.enabled = true;
        }

        if (AdminDialogue.Instance != null)
            foreach (string line in solveDialogue)
                AdminDialogue.Instance.AdminInfo(line);

        Debug.Log("Server room puzzle solved.");
    }

    // Legacy support — kept so nothing breaks if called externally
    public void CheckAllConnected() => OnCableConnected();
}