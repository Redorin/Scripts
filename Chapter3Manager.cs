using UnityEngine;
using System.Collections;

// Master controller for Chapter 3.
// Attach to an empty GameObject called "Chapter3Manager" inside the Chapter3 parent.
// Chapter 3 is the glitched version of Chapter 1 - same rooms, broken reality.

public class Chapter3Manager : MonoBehaviour
{
    [Header("Chapter 3 Root")]
    public GameObject chapter3Root;         // The Chapter3 parent object

    [Header("Player")]
    public GameObject playerBody;           // Player > Body - hide this for no-body effect

    [Header("Glitch Objects")]
    public GlitchFlicker[] glitchObjects;   // All objects with GlitchFlicker attached
    public Light[] roomLights;              // Lights to flicker

    [Header("Students")]
    public GameObject[] studentObjects;     // Frozen student models

    [Header("ADMIN ACCESS Door")]
    public GameObject adminAccessDoor;      // Special door that appears at end
    public Transform adminAccessSpawnPoint; // Where it appears

    [Header("Void Floor")]
    public GameObject voidFloor;            // Floor that disappears in forced event
    public float voidFloorDelay = 5f;       // Time before floor disappears

    [Header("Dialogue - Chapter Start")]
    public string[] startDialogue = {
        "Sector integrity: degraded.",
        "Returning observer to initialization point.",
        "Warning: rollback authority limited."
    };

    [Header("Dialogue - Try Reset Student")]
    public string[] studentResetDialogue = {
        "Biological entities outside rollback authority.",
        "Correction denied."
    };

    [Header("Dialogue - Forced Instability")]
    public string[] forcedInstabilityDialogue = {
        "Critical failure.",
        "Forced reset imminent.",
        "ADMIN ACCESS required."
    };

    [Header("State")]
    public bool chapterStarted = false;
    public bool forcedEventTriggered = false;

    public static Chapter3Manager Instance;

    void Awake()
    {
        Instance = this;
    }

    // Called by ChapterTransition when moving from Chapter 2 to Chapter 3
    public void StartChapter()
    {
        if (chapterStarted) return;
        chapterStarted = true;
        StartCoroutine(ChapterStartSequence());
    }

    IEnumerator ChapterStartSequence()
    {
        // Hide player body (no-body effect)
        if (playerBody != null)
            playerBody.SetActive(false);

        // Start all glitch effects
        foreach (GlitchFlicker g in glitchObjects)
        {
            if (g != null) g.StartGlitch();
        }

        yield return new WaitForSeconds(1f);

        // Play start dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in startDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }
    }

    // Called when player tries to reset a student
    public void OnStudentResetAttempt()
    {
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in studentResetDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }
    }

    // Called by trigger zone in the hallway
    public void TriggerForcedInstabilityEvent()
    {
        if (forcedEventTriggered) return;
        forcedEventTriggered = true;
        StartCoroutine(ForcedInstabilitySequence());
    }

    IEnumerator ForcedInstabilitySequence()
    {
        // Spike instability to critical
        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(50f);

        // Play dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in forcedInstabilityDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        yield return new WaitForSeconds(2f);

        // Floor disappears
        if (voidFloor != null)
        {
            yield return new WaitForSeconds(voidFloorDelay);
            voidFloor.SetActive(false);
        }

        yield return new WaitForSeconds(1f);

        // ADMIN ACCESS door appears
        if (adminAccessDoor != null)
        {
            adminAccessDoor.SetActive(true);
        }
    }
}