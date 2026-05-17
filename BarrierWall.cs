// File: BarrierWall.cs
// ============================================================================
// Automatically added to each invisible wall by DebrisBarrier.
// When the player aims the Reset Device at a wall and presses R,
// ResetItem hits this via GetComponentInParent<ResettableObject>()...
// But since we're not using ResettableObject here, we hook into
// PlayerInteraction instead via a new interactable type.
//
// Actually — this uses a simpler approach:
// BarrierWall has a ResettableObject sibling that calls back to DebrisBarrier.
// No changes needed to ResetItem.cs.
// ============================================================================
using UnityEngine;

[RequireComponent(typeof(ResettableObject))]
public class BarrierWall : MonoBehaviour
{
    [HideInInspector]
    public DebrisBarrier barrier;

    private ResettableObject resettable;

    void Awake()
    {
        resettable = GetComponent<ResettableObject>();

        // Configure the ResettableObject to not move this wall,
        // just fire the callback
        resettable.objectName = "Debris Barrier";
        resettable.canBeReset = true;
        resettable.resetPosition = false;
        resettable.resetRotation = false;
        resettable.resetScale = false;
        resettable.resetPhysics = false;
        resettable.resetActiveState = false;
        resettable.unlimitedUses = false;
        resettable.maxResetUses = 1;

        // Subscribe to the reset event
        resettable.OnResetSuccess += OnWallReset;
    }

    void OnWallReset()
    {
        if (barrier != null)
            barrier.TriggerDebrisReset();
    }

    void OnDestroy()
    {
        if (resettable != null)
            resettable.OnResetSuccess -= OnWallReset;
    }
}