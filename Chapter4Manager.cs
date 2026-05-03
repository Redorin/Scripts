using UnityEngine;
using System.Collections;
using TMPro;

// Master controller for Chapter 4.
// Admin voice splits into Admin A (Stability) and Admin B (Truth).
// Player choices align them with one Admin.
// Attach to empty GameObject "Chapter4Manager" inside Chapter4 parent.

public class Chapter4Manager : MonoBehaviour
{
    [Header("Admin A - Stability (Blue)")]
    public GameObject adminAPanel;
    public TextMeshProUGUI adminAText;
    public Color adminAColor = new Color(0f, 0.7f, 1f, 1f);    // Cyan/blue

    [Header("Admin B - Truth (Red/Orange)")]
    public GameObject adminBPanel;
    public TextMeshProUGUI adminBText;
    public Color adminBColor = new Color(1f, 0.3f, 0f, 1f);    // Red/orange

    [Header("Alignment Tracking")]
    public int adminAScore = 0;         // Increases when player sides with stability
    public int adminBScore = 0;         // Increases when player sides with truth

    [Header("Final Choice Corridors")]
    public GameObject corridorA;        // Admin A corridor - leads to stability ending
    public GameObject corridorB;        // Admin B corridor - leads to truth ending
    public float corridorRevealDelay = 2f;

    [Header("Classroom")]
    public GameObject[] classroomStudents;  // Students that rotate heads unnaturally
    public float headRotateSpeed = 2f;

    [Header("Reset Device Pedestal")]
    public GameObject resetDevicePedestal;  // Pedestal Reset Device appears on at end
    public GameObject resetDeviceObject;    // The actual Reset Device prop

    [Header("Dialogue - Chapter Start")]
    public string[] startDialogueA = {
        "Observer detected. Stability protocols active.",
        "Remain within parameters."
    };
    public string[] startDialogueB = {
        "You can hear me now.",
        "They don't want you to know what this place is."
    };

    [Header("State")]
    public bool chapterStarted = false;
    public bool finalChoiceMade = false;

    public static Chapter4Manager Instance;

    void Awake()
    {
        Instance = this;

        // Hide panels at start
        if (adminAPanel != null) adminAPanel.SetActive(false);
        if (adminBPanel != null) adminBPanel.SetActive(false);

        // Hide corridors until end
        if (corridorA != null) corridorA.SetActive(false);
        if (corridorB != null) corridorB.SetActive(false);

        // Hide reset device on pedestal until end
        if (resetDeviceObject != null) resetDeviceObject.SetActive(false);
    }

    public void StartChapter()
    {
        if (chapterStarted) return;
        chapterStarted = true;
        StartCoroutine(ChapterStartSequence());
    }

    IEnumerator ChapterStartSequence()
    {
        yield return new WaitForSeconds(1f);

        // Admin A speaks first
        ShowAdminA(startDialogueA);

        yield return new WaitForSeconds(3f);

        // Admin B cuts in
        ShowAdminB(startDialogueB);

        // Start student head rotation
        StartCoroutine(RotateStudentHeads());
    }

    // Show Admin A dialogue
    public void ShowAdminA(string[] lines)
    {
        StartCoroutine(ShowAdminPanel(adminAPanel, adminAText, adminAColor, lines));
    }

    // Show Admin B dialogue
    public void ShowAdminB(string[] lines)
    {
        StartCoroutine(ShowAdminPanel(adminBPanel, adminBText, adminBColor, lines));
    }

    IEnumerator ShowAdminPanel(GameObject panel, TextMeshProUGUI text, Color color, string[] lines)
    {
        if (panel == null || text == null) yield break;

        panel.SetActive(true);
        text.color = color;

        foreach (string line in lines)
        {
            text.text = line;
            yield return new WaitForSeconds(3f);
        }

        panel.SetActive(false);
    }

    // Called by puzzle objects when player makes a choice
    // isStability = true means player sided with Admin A
    public void RegisterChoice(bool isStability)
    {
        if (isStability)
        {
            adminAScore++;
            ShowAdminA(new string[] { "Correct. Maintain order." });
            ShowAdminB(new string[] { "You're choosing blindness." });
        }
        else
        {
            adminBScore++;
            ShowAdminB(new string[] { "Good. Keep looking." });
            ShowAdminA(new string[] { "Deviation logged." });
        }

        // Check if enough choices made to reveal final corridors
        int totalChoices = adminAScore + adminBScore;
        if (totalChoices >= 3 && !finalChoiceMade)
        {
            StartCoroutine(RevealFinalChoice());
        }
    }

    IEnumerator RevealFinalChoice()
    {
        finalChoiceMade = true;

        yield return new WaitForSeconds(corridorRevealDelay);

        // Reveal both corridors
        if (corridorA != null) corridorA.SetActive(true);
        if (corridorB != null) corridorB.SetActive(true);

        ShowAdminA(new string[] { "Choose stability. Choose the left." });
        ShowAdminB(new string[] { "Choose truth. Choose the right." });
    }

    // Called when player enters a corridor
    public void OnCorridorChosen(bool choseStability)
    {
        StartCoroutine(CorridorEndSequence(choseStability));
    }

    IEnumerator CorridorEndSequence(bool choseStability)
    {
        if (choseStability)
        {
            ShowAdminA(new string[] {
                "Correct choice.",
                "Stability preserved.",
                "Initiating correction protocol."
            });
        }
        else
        {
            ShowAdminB(new string[] {
                "Now you know.",
                "The system was never yours.",
                "It was always watching."
            });
        }

        yield return new WaitForSeconds(4f);

        // Reset Device appears on pedestal - auto activates
        if (resetDeviceObject != null)
            resetDeviceObject.SetActive(true);

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminWarning("Rollback device reinitialized.");
    }

    // Placeholder head rotation - rotate student transforms unnaturally
    IEnumerator RotateStudentHeads()
    {
        if (classroomStudents == null) yield break;

        float elapsed = 0f;
        float duration = 3f;

        while (elapsed < duration)
        {
            foreach (GameObject student in classroomStudents)
            {
                if (student != null)
                {
                    // Slowly rotate head (whole body as placeholder)
                    student.transform.Rotate(Vector3.up * headRotateSpeed * Time.deltaTime);
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}