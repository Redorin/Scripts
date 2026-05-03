using UnityEngine;

// Attach to a trigger zone at the entrance of each corridor in Chapter 4.
// When player walks in, registers their final choice.

public class CorridorChoice : MonoBehaviour
{
    [Header("Which corridor is this")]
    public bool isStabilityCoridor = true;  // True = Admin A, False = Admin B

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (Chapter4Manager.Instance != null)
            Chapter4Manager.Instance.OnCorridorChosen(isStabilityCoridor);
    }
}