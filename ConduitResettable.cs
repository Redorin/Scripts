using UnityEngine;

public class ConduitResettable : MonoBehaviour
{
    [Header("References")]
    public BreakerPanelPuzzle breakerPanel;

    public void OnConduitReset()
    {
        if (breakerPanel != null)
            breakerPanel.SetConduitReset(true);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(
                "Conduit restored. Power path clear.");
    }
}