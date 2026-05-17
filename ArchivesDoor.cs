using UnityEngine;

public class ArchivesDoor : MonoBehaviour
{
    [Header("Key Settings")]
    public string requiredKeyName = "ArchiveKey";

    [Header("Dialogue")]
    public string lockedDialogue = "Door sealed. An access key is required.";
    public string[] unlockDialogue = {
        "Key accepted.",
        "Archives unlocked."
    };

    [Header("State")]
    public bool isUnlocked = false;

    private InteractableDoor door;
    private ItemHolder itemHolder;

    void Start()
    {
        door = GetComponent<InteractableDoor>();
        if (door != null) door.enabled = false;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            itemHolder = player.GetComponent<ItemHolder>();
    }

    public void TryOpen()
    {
        if (isUnlocked)
        {
            if (door != null) door.Interact();
            return;
        }

        // Must be currently holding the key
        bool hasKey = itemHolder != null &&
                      itemHolder.GetCurrentItemName() == requiredKeyName;

        if (hasKey)
            Unlock();
        else
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(lockedDialogue);
    }

    void Unlock()
    {
        isUnlocked = true;
        if (door != null)
        {
            door.enabled = true;
            door.Interact();
        }

        if (AdminDialogue.Instance != null)
            foreach (string line in unlockDialogue)
                AdminDialogue.Instance.AdminWarning(line);
    }
}