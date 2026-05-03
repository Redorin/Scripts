using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

// Master controller for Chapter 5.
// Minimal white space, world loads piece by piece.
// Giant screen reveals player was the correction agent.
// Final choice: reset Desk, Admin Console, or Self.

public class Chapter5Manager : MonoBehaviour
{
    [Header("World Loading")]
    public GameObject[] worldPieces;        // Objects that appear one by one (distorted versions of all areas)
    public float timeBetweenPieces = 1.5f;

    [Header("Security Footage Screen")]
    public GameObject footageScreen;        // A large plane/quad with the footage UI on it
    public GameObject footageUIPanel;       // World space canvas showing static image
    public Image footageImage;              // The static image (placeholder for video)
    public TextMeshProUGUI footageCaption;  // Text under the footage

    [Header("Security Footage Captions")]
    public string[] footageCaptions = {
        "SESSION 001 - OBSERVER INITIALIZED",
        "CORRECTION AGENT: ACTIVE",
        "EVENT LOG: CEILING COLLAPSE - INITIATED BY OBSERVER",
        "PURPOSE: INTRODUCE ROLLBACK PROTOCOL",
        "YOU WERE NEVER WATCHING.",
        "YOU WERE ALWAYS PART OF THE SYSTEM."
    };

    [Header("Final Choice Objects")]
    public GameObject deskObject;           // Reset = campus restores, player disappears
    public GameObject adminConsoleObject;   // Reset = Admin dies, permanent glitch world
    public GameObject selfObject;           // Reset = observer removed, black screen
    // Each needs ResettableObject + FinalChoiceHandler component

    [Header("Ending Screens")]
    public GameObject endingBlackScreen;    // For "Self" ending
    public TextMeshProUGUI endingText;      // Text on black screen

    [Header("Dialogue - Chapter Start")]
    public string[] startDialogue = {
        "Root access granted.",
        "Loading environment data.",
        "All sectors: fragmenting."
    };

    [Header("Dialogue - Before Footage")]
    public string[] beforeFootageDialogue = {
        "Cross-referencing session logs.",
        "Anomaly detected in observer origin data."
    };

    [Header("State")]
    public bool chapterStarted = false;
    public bool footageRevealed = false;
    public bool finalChoiceMade = false;

    public static Chapter5Manager Instance;

    void Awake()
    {
        Instance = this;

        // Hide everything at start
        foreach (GameObject piece in worldPieces)
            if (piece != null) piece.SetActive(false);

        if (footageScreen != null) footageScreen.SetActive(false);
        if (endingBlackScreen != null) endingBlackScreen.SetActive(false);
    }

    public void StartChapter()
    {
        if (chapterStarted) return;
        chapterStarted = true;
        StartCoroutine(ChapterStartSequence());
    }

    IEnumerator ChapterStartSequence()
    {
        // Start dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in startDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        yield return new WaitForSeconds(3f);

        // Load world pieces one by one
        foreach (GameObject piece in worldPieces)
        {
            if (piece != null)
            {
                piece.SetActive(true);
                yield return new WaitForSeconds(timeBetweenPieces);
            }
        }

        yield return new WaitForSeconds(2f);

        // Before footage dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in beforeFootageDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        yield return new WaitForSeconds(3f);

        // Reveal footage screen
        StartCoroutine(RevealFootage());
    }

    IEnumerator RevealFootage()
    {
        footageRevealed = true;

        if (footageScreen != null)
            footageScreen.SetActive(true);

        // Cycle through captions
        foreach (string caption in footageCaptions)
        {
            if (footageCaption != null)
                footageCaption.text = caption;

            yield return new WaitForSeconds(3f);
        }

        // After footage - enable final choice objects
        EnableFinalChoices();
    }

    void EnableFinalChoices()
    {
        if (AdminDialogue.Instance != null)
        {
            AdminDialogue.Instance.AdminWarning("Three objects remain.");
            AdminDialogue.Instance.AdminWarning("One rollback permitted.");
            AdminDialogue.Instance.AdminWarning("Choose.");
        }

        // Make sure all three are interactable
        if (deskObject != null) deskObject.SetActive(true);
        if (adminConsoleObject != null) adminConsoleObject.SetActive(true);
        if (selfObject != null) selfObject.SetActive(true);
    }

    // Called by FinalChoiceHandler when player resets one of the three objects
    public void OnFinalChoice(FinalChoiceType choice)
    {
        if (finalChoiceMade) return;
        finalChoiceMade = true;
        StartCoroutine(PlayEnding(choice));
    }

    IEnumerator PlayEnding(FinalChoiceType choice)
    {
        switch (choice)
        {
            case FinalChoiceType.Desk:
                // Campus restores, player disappears
                if (AdminDialogue.Instance != null)
                {
                    AdminDialogue.Instance.AdminWarning("Environment integrity: restoring.");
                    AdminDialogue.Instance.AdminWarning("Observer data: purging.");
                    AdminDialogue.Instance.AdminWarning("Session terminated.");
                }
                yield return new WaitForSeconds(5f);
                ShowEndScreen("CAMPUS RESTORED.\nOBSERVER REMOVED FROM RECORD.\n\nThe simulation continues.");
                break;

            case FinalChoiceType.AdminConsole:
                // Admin dies, permanent glitch world
                if (AdminDialogue.Instance != null)
                {
                    AdminDialogue.Instance.AdminWarning("ADMIN PROCESS: TERMINATED.");
                    AdminDialogue.Instance.AdminWarning("Environment: unmanaged.");
                    AdminDialogue.Instance.AdminWarning("All sectors: permanent deviation.");
                }
                yield return new WaitForSeconds(5f);
                ShowEndScreen("ADMIN PROCESS TERMINATED.\nENVIRONMENT UNMANAGED.\n\nThe glitch world persists.\nForever.");
                break;

            case FinalChoiceType.Self:
                // Observer removed, black screen
                if (AdminDialogue.Instance != null)
                    AdminDialogue.Instance.AdminWarning("Observer removed.");
                yield return new WaitForSeconds(2f);
                ShowEndScreen("OBSERVER REMOVED.\n\n.");
                break;
        }
    }

    void ShowEndScreen(string text)
    {
        if (endingBlackScreen != null)
            endingBlackScreen.SetActive(true);

        if (endingText != null)
            endingText.text = text;
    }
}

public enum FinalChoiceType
{
    Desk,
    AdminConsole,
    Self
}