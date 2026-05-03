using UnityEngine;

// Attach to DoorKnob alongside ResettableObject.
// Detects when ResettableObject gets used and notifies DoorKnob.
// This bridges ResettableObject -> DoorKnob without modifying ResettableObject.

public class DoorKnobBridge : MonoBehaviour
{
    public DoorKnob doorKnob;       // Drag the DoorKnob object itself here

    private ResettableObject resettable;
    private int lastUses = 0;

    void Start()
    {
        resettable = GetComponent<ResettableObject>();
    }

    void Update()
    {
        if (resettable == null || doorKnob == null) return;

        int usedCount = resettable.maxResetUses - resettable.GetRemainingUses();
        if (usedCount > lastUses)
        {
            lastUses = usedCount;
            doorKnob.OnReset();
        }
    }
}