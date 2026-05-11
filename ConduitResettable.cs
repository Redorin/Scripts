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

        // If all parts are reset (or no parts assigned, treat as single object)
        int required = conduitParts != null && conduitParts.Length > 0
            ? conduitParts.Length
            : 1;

        if (resetCount >= required)
        {
            if (breakerPanel != null)
                breakerPanel.SetConduitReset(true);

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(
                    "Conduit restored. Power path clear.");

                    Chapter1Objectives.Instance?.Complete_ResetConduit();
        }
        else
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo(
                    "Partial restoration. Conduit still damaged.");
        }
    }
}