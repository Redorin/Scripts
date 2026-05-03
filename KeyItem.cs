using UnityEngine;

// Attach to the Key object in the Server Room.
// Player picks it up with E (HoldableItem handles the pickup).
// ArchivesDoor checks if player has this key before unlocking.

public class KeyItem : MonoBehaviour
{
    [Header("Key Info")]
    public string keyName = "Archives Key";
    public bool isPickedUp = false;

    // Singleton-style static reference so ArchivesDoor can check it easily
    public static KeyItem Instance;

    void Awake()
    {
        Instance = this;
    }

    // Called by HoldableItem when player picks it up
    // We detect pickup by checking if HoldableItem.IsBeingHeld() is true
    void Update()
    {
        HoldableItem holdable = GetComponent<HoldableItem>();
        if (holdable != null && holdable.IsBeingHeld() && !isPickedUp)
        {
            isPickedUp = true;

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Key acquired. Find the locked door.");

            Debug.Log(keyName + " picked up.");
        }
    }
}