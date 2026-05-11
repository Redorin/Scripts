using UnityEngine;

// Attach to any object that should be locked until a later chapter/event.
// PlayerInteraction will need to be updated to detect this, or use InteractableObject
// with a custom message and this script alongside it.

public class LockedObject : MonoBehaviour
{
    [Header("Lock Settings")]
    public bool isLocked = true;
    public string lockedMessage    = "Access restricted.";
    public string unlockedMessage  = "Opening...";

    [Header("On Unlock")]
    // Optional: GameObject to activate when unlocked (open drawer mesh, etc.)
    public GameObject lockedVisual;    // shown when locked
    public GameObject unlockedVisual;  // shown when unlocked

    [Header("Unlock Trigger")]
    // Leave empty to unlock via script only (e.g. from Chapter 3 manager)
    public string unlockInChapter = "Chapter3";

    void Start()
    {
        UpdateVisual();
    }

    public void Interact()
    {
        if (isLocked)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(lockedMessage);
            return;
        }

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(unlockedMessage);

        // Play open animation or activate content
        UpdateVisual();
    }

    public void Unlock()
    {
        isLocked = false;
        UpdateVisual();

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo("Restricted access granted.");
    }

    void UpdateVisual()
    {
        if (lockedVisual   != null) lockedVisual.SetActive(isLocked);
        if (unlockedVisual != null) unlockedVisual.SetActive(!isLocked);
    }
}