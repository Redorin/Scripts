using UnityEngine;

// Attach to a door that requires a keycard.
// Works alongside InteractableDoor — disable InteractableDoor until keycard is used.

public class KeycardDoor : MonoBehaviour
{
    [Header("Keycard Settings")]
    public string requiredKeycardName = "ArzatechKeycard";

    [Header("Dialogue")]
    public string noKeycardMessage  = "Keycard required for access.";
    public string wrongCardMessage  = "Invalid keycard.";
    public string accessGranted     = "Access granted. Welcome to Arzatech Server Room.";

    [Header("State")]
    public bool isUnlocked = false;

    private InteractableDoor door;
    private ItemHolder itemHolder;

    void Start()
    {
        door = GetComponent<InteractableDoor>();
        if (door == null)
            door = GetComponentInChildren<InteractableDoor>();

        // Lock the door until keycard is used
        if (door != null) door.enabled = false;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            itemHolder = player.GetComponent<ItemHolder>();
    }

    public void Interact()
    {
        if (isUnlocked)
        {
            if (door != null) door.Interact();
            return;
        }

        if (itemHolder == null) return;

        // Check inventory for keycard
        bool hasCard = false;
        for (int i = 0; i < itemHolder.GetInventoryCount(); i++)
        {
            if (itemHolder.GetItemNameAtIndex(i) == requiredKeycardName)
            {
                hasCard = true;
                break;
            }
        }

        if (hasCard)
        {
            Unlock();
        }
        else
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(noKeycardMessage);
        }
    }

    void Unlock()
    {
        isUnlocked = true;

        if (door != null) door.enabled = true;

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(accessGranted);

        // Open immediately on first use
        if (door != null) door.Interact();
    }
}