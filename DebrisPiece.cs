using UnityEngine;

// Attach to each individual debris cube.
// When the Reset Device hits this piece, it tells the DebrisGroup
// to reset ALL pieces at once instead of just this one.

public class DebrisPiece : MonoBehaviour
{
    [Header("Parent Group")]
    public DebrisGroup debrisGroup;     // Drag the parent DebrisGroup object here

    // Called by ResetItem when player aims at this piece and presses R
    // We intercept this BEFORE ResettableObject.Reset() is called
    public void NotifyGroup()
    {
        if (debrisGroup != null)
            debrisGroup.ResetAll();
    }
}