using UnityEngine;

// Attach to Desk, AdminConsole, and Self (camera object) in Chapter 5.
// When player resets this object, it triggers the corresponding ending.
// Works alongside ResettableObject - set canBeReset = true, maxResetUses = 1.

public class FinalChoiceHandler : MonoBehaviour
{
    [Header("Which ending this triggers")]
    public FinalChoiceType choiceType = FinalChoiceType.Desk;

    [Header("Dialogue shown when player looks at this")]
    public string hoverDescription = "A desk. Familiar.";

    private bool hasBeenChosen = false;

    // Called by ResetItem when this object is reset
    // Hook this up: in ResettableObject on this object, it will call Reset()
    // Add this to the reset callback by modifying ResetItem or using SendMessage

    public void OnReset()
    {
        if (hasBeenChosen) return;
        hasBeenChosen = true;

        if (Chapter5Manager.Instance != null)
            Chapter5Manager.Instance.OnFinalChoice(choiceType);
    }
}