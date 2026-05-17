// File: BreakerPanelPuzzle.cs
using UnityEngine;

public class BreakerPanelPuzzle : MonoBehaviour
{
    [Header("Breaker Settings")]
    public BreakerSwitch[] breakers;
    public int[] correctOrder = { 0, 2, 1 };

    [Header("References")]
    public FuseBoxPuzzle fuseBox;
    public GameObject unlockedDoor;

    [Header("State")]
    public bool fuseReady = false;
    public bool conduitReset = false;
    public bool puzzleSolved = false;

    private int currentStep = 0;

    void Start()
    {
        foreach (BreakerSwitch b in breakers)
            if (b != null) b.SetInteractable(false);
    }

    public void SetFuseReady(bool ready)
    {
        fuseReady = ready;
        CheckRequirements();
    }

    public void SetConduitReset(bool reset)
    {
        conduitReset = reset;
        CheckRequirements();
    }

    void CheckRequirements()
    {
        if (fuseReady && conduitReset)
        {
            foreach (BreakerSwitch b in breakers)
                if (b != null) b.SetInteractable(true);

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Panel ready. Sequential activation required.");
        }
    }

    public void OnBreakerFlipped(int breakerIndex)
    {
        if (puzzleSolved) return;
        if (!fuseReady || !conduitReset) return;

        if (breakerIndex == correctOrder[currentStep])
        {
            currentStep++;

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminInfo("Step " + currentStep + " complete.");

            if (currentStep >= correctOrder.Length)
                PuzzleSolved();
        }
        else
        {
            ResetBreakers();

            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning("Sequence violation. Failsafe triggered.");
        }
    }

    void ResetBreakers()
    {
        currentStep = 0;
        foreach (BreakerSwitch b in breakers)
            if (b != null) b.ResetBreaker();
    }

    void PuzzleSolved()
    {
        puzzleSolved = true;

        // Fires ObjectiveTrigger on Breaker GameObject
        GetComponent<ObjectiveTrigger>()?.TriggerObjective();

        if (fuseBox != null)
            fuseBox.PowerOn();

        if (unlockedDoor != null)
        {
            InteractableDoor door = unlockedDoor.GetComponent<InteractableDoor>();
            if (door != null) door.enabled = true;
        }

        if (AdminDialogue.Instance != null)
        {
            AdminDialogue.Instance.AdminInfo("Power sequence complete.");
            AdminDialogue.Instance.AdminInfo("Access granted.");
        }
    }
}