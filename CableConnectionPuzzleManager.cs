// File: CableConnectionPuzzleManager.cs
using UnityEngine;

public class CableConnectionPuzzleManager : MonoBehaviour
{
    [Header("Cables")]
    public CableConnectionPuzzle[] cables;

    [Header("Server Boot Order")]
    public ServerTerminal[] servers;
    public bool isSolved = false;

    [Header("Lockdown")]
    public ServerRoomLockdown lockdown;

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
    private int connectionsComplete = 0;

    // Called by each CableConnectionPuzzle when it connects to a WallPlug
    public void OnCableConnected()
    {
        if (cablesComplete) return;

        connectionsComplete++;

        int required = cables != null ? cables.Length : 2;
        if (connectionsComplete < required) return;

        cablesComplete = true;

        // Fire first ObjectiveTrigger — completes connect_cables, adds boot_servers
        ObjectiveTrigger[] triggers = GetComponents<ObjectiveTrigger>();
        if (triggers.Length > 0) triggers[0].TriggerObjective();

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

        if (servers[currentBootStep] == server)
        {
            server.SetOnline();
            currentBootStep++;

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Server " + currentBootStep + " online.");

            if (currentBootStep >= servers.Length)
                SolvePuzzle();
        }
        else
        {
            currentBootStep = 0;
            foreach (ServerTerminal s in servers)
                if (s != null) s.ResetBoot();

            if (InstabilityManager.Instance != null)
                InstabilityManager.Instance.IncreaseInstability(10f);

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning("Boot sequence error. Instability increased.");
        }
    }

    void SolvePuzzle()
    {
        isSolved = true;

        // Fire second ObjectiveTrigger — completes boot_servers, adds retrieve_archive_key
        ObjectiveTrigger[] triggers = GetComponents<ObjectiveTrigger>();
        if (triggers.Length > 1) triggers[1].TriggerObjective();

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
    }

    public void CheckAllConnected() => OnCableConnected();
}