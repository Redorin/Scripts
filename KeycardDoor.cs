// File: KeycardDoor.cs
using UnityEngine;

public class KeycardDoor : MonoBehaviour
{
    [Header("Keycard Settings")]
    public string requiredKeycardName = "ArzatechKeycard";

    [Header("Dialogue")]
    public string noKeycardMessage = "Keycard required for access.";
    public string accessGranted    = "Access granted. Welcome to Arzatech Server Room.";

    [Header("State")]
    public bool isUnlocked = false;

    private InteractableDoor door;
    private ItemHolder itemHolder;
    private bool keycardObjectiveShown = false;

    void Start()
    {
        door = GetComponent<InteractableDoor>();
        if (door == null)
            door = GetComponentInChildren<InteractableDoor>();

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

        // Check if keycard is anywhere in inventory
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
            // If find_keycard is already complete, player already had it
            // Just unlock without adding use_keycard objective
            bool alreadyKnewAboutKeycard = ObjectiveManager.Instance != null &&
                ObjectiveManager.Instance.IsComplete("find_keycard");

            if (!alreadyKnewAboutKeycard)
            {
                // Player found the keycard and is now using it
                ObjectiveManager.Instance?.Add("use_keycard");
                ObjectiveManager.Instance?.Complete("use_keycard");
            }

            Unlock();
        }
        else
        {
            // Player doesn't have the keycard — tell them to find it
            if (!keycardObjectiveShown)
            {
                keycardObjectiveShown = true;
                ObjectiveManager.Instance?.Add("find_keycard");
            }

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

        if (door != null) door.Interact();
    }
}