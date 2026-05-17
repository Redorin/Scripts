// File: ServerRoomLockdown.cs
using UnityEngine;

public class ServerRoomLockdown : MonoBehaviour
{
    [Header("References")]
    public InteractableDoor exitDoor;
    public KeycardDoor keycardDoor;
    public CableConnectionPuzzleManager puzzleManager;
    public KeySpawner keySpawner;

    [Header("Dialogue")]
    public string lockdownMessage = "Emergency lockdown initiated. Complete diagnostics to restore access.";
    public string[] entryDialogue = {
        "Arzatech server grid — offline.",
        "Identify fault and restore connection."
    };

    private bool hasTriggered = false;
    private bool isReady = false;

    void Start()
    {
        if (exitDoor != null) exitDoor.enabled = false;
        StartCoroutine(ActivateNextFrame());
    }

    System.Collections.IEnumerator ActivateNextFrame()
    {
        yield return null;
        isReady = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isReady) return;
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        TriggerLockdown();
    }

    void TriggerLockdown()
    {
        if (exitDoor != null)    exitDoor.enabled       = false;
        if (keycardDoor != null) keycardDoor.isUnlocked = false;

        GetComponent<ObjectiveTrigger>()?.TriggerObjective();

        if (AdminDialogue.Instance != null)
        {
            AdminDialogue.Instance.AdminWarning(lockdownMessage);
            foreach (string line in entryDialogue)
                AdminDialogue.Instance.AdminInfo(line);
        }
    }

    public void Unlock()
    {
        if (exitDoor != null)    exitDoor.enabled       = true;
        if (keycardDoor != null) keycardDoor.isUnlocked = true;

        if (keySpawner != null) keySpawner.SpawnKey();

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo("Lockdown lifted. Exit restored.");
    }
}