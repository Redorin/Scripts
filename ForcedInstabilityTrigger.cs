using UnityEngine;

// Attach to an invisible trigger zone in Chapter 3 hallway.
// When player walks through, fires the forced instability event.

public class ForcedInstabilityTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (Chapter3Manager.Instance != null)
            Chapter3Manager.Instance.TriggerForcedInstabilityEvent();
    }
}