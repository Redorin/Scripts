using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("HUD — appears briefly when objective added/completed")]
    public CanvasGroup hudPanel;            // fades in/out
    public TextMeshProUGUI hudText;         // shows the new/completed objective
    public float hudDisplayDuration = 10f;
    public float hudFadeDuration = 1f;

    [Header("Pause Menu Objectives Panel")]
    public GameObject objectivesPanel;      // right side panel in pause menu
    public Transform objectiveListParent;   // vertical layout group parent
    public GameObject objectiveEntryPrefab; // prefab: TextMeshProUGUI

    [Header("Colors")]
    public Color activeColor   = new Color(0f, 0.86f, 1f, 1f);   // cyan
    public Color completedColor = new Color(0.4f, 0.4f, 0.4f, 1f); // grey

    private class Objective
    {
        public string text;
        public bool isCompleted;
        public TextMeshProUGUI uiEntry;
    }

    private List<Objective> objectives = new List<Objective>();
    private Coroutine hudCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (hudPanel != null)
        {
            hudPanel.alpha = 0f;
            hudPanel.gameObject.SetActive(false);
        }
    }

    // ── PUBLIC API ──

    // Add a new objective
    public void AddObjective(string text)
    {
        Objective obj = new Objective { text = text, isCompleted = false };
        objectives.Add(obj);

        // Create UI entry in pause menu list
        if (objectiveListParent != null && objectiveEntryPrefab != null)
        {
            GameObject entry = Instantiate(objectiveEntryPrefab, objectiveListParent);
            TextMeshProUGUI tmp = entry.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = "• " + text;
                tmp.color = activeColor;
                obj.uiEntry = tmp;
            }
        }

        ShowHUD("• " + text, false);
    }

    // Complete an existing objective by exact text match
    public void CompleteObjective(string text)
    {
        foreach (Objective obj in objectives)
        {
            if (obj.text == text && !obj.isCompleted)
            {
                obj.isCompleted = true;

                if (obj.uiEntry != null)
                {
                    obj.uiEntry.text = "<s>✓ " + obj.text + "</s>";
                    obj.uiEntry.color = completedColor;
                }

                ShowHUD("✓ " + text, true);
                break;
            }
        }
    }

    // Add and immediately complete (for objectives that complete instantly)
    public void AddAndComplete(string text)
    {
        AddObjective(text);
        CompleteObjective(text);
    }

    // ── HUD ──

    void ShowHUD(string text, bool isCompletion)
    {
        if (hudPanel == null || hudText == null) return;

        if (hudCoroutine != null)
            StopCoroutine(hudCoroutine);

        hudCoroutine = StartCoroutine(HUDSequence(text, isCompletion));
    }

    IEnumerator HUDSequence(string text, bool isCompletion)
    {
        hudText.text = text;
        hudText.color = isCompletion ? completedColor : activeColor;

        hudPanel.gameObject.SetActive(true);

        // Fade in
        float elapsed = 0f;
        while (elapsed < hudFadeDuration)
        {
            hudPanel.alpha = Mathf.Lerp(0f, 1f, elapsed / hudFadeDuration);
            elapsed += Time.unscaledDeltaTime; // unscaled so it works when paused
            yield return null;
        }
        hudPanel.alpha = 1f;

        // Hold
        yield return new WaitForSecondsRealtime(hudDisplayDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < hudFadeDuration)
        {
            hudPanel.alpha = Mathf.Lerp(1f, 0f, elapsed / hudFadeDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        hudPanel.alpha = 0f;
        hudPanel.gameObject.SetActive(false);
        hudCoroutine = null;
    }

    // Called by PauseMenuManager to show/hide objectives panel
    public void SetObjectivesPanelVisible(bool visible)
    {
        if (objectivesPanel != null)
            objectivesPanel.SetActive(visible);
    }
}