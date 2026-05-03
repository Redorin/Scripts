using UnityEngine;
using System.Collections;

public class PowerOutageEvent : MonoBehaviour
{
    [Header("Lights")]
    public Light[] roomLights;          // All lights in the area
    public float flickerDuration = 2f;  // How long lights flicker before going out
    public float flickerSpeed = 0.1f;   // How fast they flicker

    [Header("State")]
    public bool hasTriggered = false;
    public bool powerIsOut = false;

    [Header("Dialogue")]
    public string[] outageDialogue = {
        "Power failure detected.",
        "Auxiliary systems offline."
    };

    [Header("Optional - Door to lock during outage")]
    public GameObject doorToLock;       // Door that locks when power goes out

    // Call this from a DialogueTrigger zone or directly
    public void TriggerOutage()
    {
        if (hasTriggered) return;
        hasTriggered = true;
        StartCoroutine(OutageSequence());
    }

    IEnumerator OutageSequence()
    {
        // Phase 1: Flicker lights
        float elapsed = 0f;
        bool lightsOn = true;

        while (elapsed < flickerDuration)
        {
            lightsOn = !lightsOn;
            SetLights(lightsOn);
            elapsed += flickerSpeed;
            yield return new WaitForSeconds(flickerSpeed);
        }

        // Phase 2: Lights off
        SetLights(false);
        powerIsOut = true;

        // Lock door if assigned
        if (doorToLock != null)
            doorToLock.SetActive(false);

        // Admin dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in outageDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        Debug.Log("Power outage triggered.");
    }

    // Called by FuseBoxPuzzle when power is restored
    public void RestorePower()
    {
        if (!powerIsOut) return;
        powerIsOut = false;
        SetLights(true);

        if (doorToLock != null)
            doorToLock.SetActive(true);

        Debug.Log("Power restored.");
    }

    void SetLights(bool on)
    {
        if (roomLights == null) return;
        foreach (Light l in roomLights)
        {
            if (l != null) l.enabled = on;
        }
    }

    // Trigger zone — outage fires when player walks in
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            TriggerOutage();
    }
}