// File: ConduitResettable.cs
using UnityEngine;

public class ConduitResettable : MonoBehaviour
{
    [Header("References")]
    public BreakerPanelPuzzle breakerPanel;

    [Header("Parts - assign both Cylinder children")]
    public ResettableObject[] conduitParts;

    private int resetCount = 0;

    public void OnConduitReset()
    {
        resetCount++;

        int required = conduitParts != null && conduitParts.Length > 0
            ? conduitParts.Length : 1;

        if (resetCount >= required)
        {
            if (breakerPanel != null)
                breakerPanel.SetConduitReset(true);

            // Fire objective trigger — completes reset_conduit, adds activate_breakers
            GetComponent<ObjectiveTrigger>()?.TriggerObjective();

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Conduit restored. Power path clear.");
        }
        else
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Partial restoration. Conduit still damaged.");
        }
    }
}