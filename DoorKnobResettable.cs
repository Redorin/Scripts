using UnityEngine;

// Attach to the DoorKnob child object ALONGSIDE ResettableObject.
// When the player uses the Reset Device on the knob,
// ResetItem calls ResettableObject.Reset() which returns true.
// This script detects that and notifies BrokenDoorknob.

public class DoorKnobResettable : MonoBehaviour
{
    [Header("References")]
    public BrokenDoorknob doorknobManager;  // Drag DoorPivot (4) here

    private ResettableObject resettable;
    private int lastUseCount = 0;

    void Start()
    {
        resettable = GetComponent<ResettableObject>();
    }

    void Update()
    {
        if (resettable == null) return;

        // Detect when ResettableObject has been used (use count increased)
        int currentUses = resettable.maxResetUses - resettable.GetRemainingUses();
        if (currentUses > lastUseCount)
        {
            lastUseCount = currentUses;
            if (doorknobManager != null)
                doorknobManager.OnKnobReset();
        }
    }
}