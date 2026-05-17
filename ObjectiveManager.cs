// File: ObjectiveManager.cs
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ObjectiveDefinition
{
    public string id;
    [TextArea(1, 2)]
    public string displayText;
    public bool addOnStart = false;
    [Tooltip("Set > 0 to make this a counter objective e.g. (0/2)")]
    public int counterTarget = 0;
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    [Header("Objective Definitions")]
    public ObjectiveDefinition[] objectiveDefinitions;

    [Header("HUD — always visible on screen")]
    public CanvasGroup hudPanel;
    public TextMeshProUGUI hudText;

    [Header("Pause Menu Objectives Panel")]
    public GameObject objectivesPanel;
    public Transform objectiveListParent;
    public GameObject objectiveEntryPrefab;

    [Header("Colors")]
    public Color activeColor    = new Color(0f, 0.86f, 1f, 1f);
    public Color completedColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Completed Objective Behaviour")]
    public float completedHoldDuration = 3f;
    public float completedFadeDuration = 1f;

    private class ObjectiveState
    {
        public string id;
        public string displayText;
        public bool isCompleted;
        public bool isRemoved;
        public TextMeshProUGUI uiEntry;
        public int counterCurrent;
        public int counterTarget;
        public bool isCounter => counterTarget > 0;
    }

    private Dictionary<string, ObjectiveDefinition> definitionLookup
        = new Dictionary<string, ObjectiveDefinition>();
    private List<ObjectiveState> activeObjectives = new List<ObjectiveState>();
    private Dictionary<string, ObjectiveState> activeLookup
        = new Dictionary<string, ObjectiveState>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);

        if (objectiveDefinitions != null)
            foreach (var def in objectiveDefinitions)
                if (!string.IsNullOrEmpty(def.id))
                    definitionLookup[def.id] = def;
    }

    void Start()
    {
        if (hudPanel != null)
        {
            hudPanel.alpha = 1f;
            hudPanel.gameObject.SetActive(true);
        }

        if (objectivesPanel != null)
            objectivesPanel.SetActive(false);

        if (objectiveDefinitions != null)
            foreach (var def in objectiveDefinitions)
                if (def.addOnStart) Add(def.id);

        RefreshHUD();
    }

    // ── PUBLIC API ──

    public void Add(string id)
    {
        if (activeLookup.ContainsKey(id))
        {
            Debug.LogWarning("[ObjectiveManager] Already added: " + id);
            return;
        }

        if (!definitionLookup.TryGetValue(id, out ObjectiveDefinition def))
        {
            Debug.LogWarning("[ObjectiveManager] No definition for ID: " + id);
            return;
        }

        ObjectiveState state = new ObjectiveState
        {
            id             = id,
            displayText    = def.displayText,
            isCompleted    = false,
            isRemoved      = false,
            counterCurrent = 0,
            counterTarget  = def.counterTarget
        };

        activeObjectives.Add(state);
        activeLookup[id] = state;

        if (objectiveListParent != null && objectiveEntryPrefab != null)
        {
            GameObject entry = Instantiate(objectiveEntryPrefab, objectiveListParent);
            TextMeshProUGUI tmp = entry.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text  = FormatText(state);
                tmp.color = activeColor;
                state.uiEntry = tmp;
            }
        }

        RefreshHUD();
    }

    public void Complete(string id)
    {
        if (!activeLookup.TryGetValue(id, out ObjectiveState state))
        {
            Debug.LogWarning("[ObjectiveManager] Not found or not added: " + id);
            return;
        }

        if (state.isCompleted) return;

        state.isCompleted = true;

        if (state.uiEntry != null)
        {
            state.uiEntry.text  = "<s>✓ " + state.displayText + "</s>";
            state.uiEntry.color = completedColor;
        }

        RefreshHUD();
        StartCoroutine(FadeOutCompleted(state));
    }

    // Increment counter objective — auto-completes when target reached
    public void IncrementCounter(string id)
    {
        if (!activeLookup.TryGetValue(id, out ObjectiveState state))
        {
            Debug.LogWarning("[ObjectiveManager] Counter objective not found: " + id);
            return;
        }

        if (state.isCompleted) return;
        if (!state.isCounter) return;

        state.counterCurrent++;
        state.counterCurrent = Mathf.Min(state.counterCurrent, state.counterTarget);

        if (state.uiEntry != null)
            state.uiEntry.text = FormatText(state);

        RefreshHUD();

        if (state.counterCurrent >= state.counterTarget)
            Complete(id);
    }

    public void AddAndComplete(string id)
    {
        Add(id);
        Complete(id);
    }

    public bool IsComplete(string id)
    {
        if (activeLookup.TryGetValue(id, out ObjectiveState state))
            return state.isCompleted;
        return false;
    }

    public bool IsAdded(string id) => activeLookup.ContainsKey(id);

    // ── FORMATTING ──

    string FormatText(ObjectiveState state)
    {
        if (state.isCounter)
            return "• " + state.displayText + " (" + state.counterCurrent + "/" + state.counterTarget + ")";
        return "• " + state.displayText;
    }

    // ── FADE OUT ──

    IEnumerator FadeOutCompleted(ObjectiveState state)
    {
        yield return new WaitForSeconds(completedHoldDuration);

        if (state.uiEntry != null)
        {
            float elapsed = 0f;
            Color startColor = state.uiEntry.color;

            while (elapsed < completedFadeDuration)
            {
                float t = elapsed / completedFadeDuration;
                if (state.uiEntry != null)
                    state.uiEntry.color = Color.Lerp(startColor, Color.clear, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (state.uiEntry != null)
            {
                Destroy(state.uiEntry.gameObject);
                state.uiEntry = null;
            }
        }

        state.isRemoved = true;
        activeObjectives.Remove(state);
        activeLookup.Remove(state.id);

        RefreshHUD();
    }

    // ── HUD ──

    void RefreshHUD()
    {
        if (hudPanel == null || hudText == null) return;

        bool hasActive = false;
        string display = "<b>OBJECTIVES</b>\n";

        foreach (ObjectiveState state in activeObjectives)
        {
            if (state.isRemoved) continue;
            hasActive = true;

            if (state.isCompleted)
                display += "<color=#666666><s>✓ " + state.displayText + "</s></color>\n";
            else if (state.isCounter)
                display += "<color=#00DBFF>• " + state.displayText +
                           " (" + state.counterCurrent + "/" + state.counterTarget + ")</color>\n";
            else
                display += "<color=#00DBFF>• " + state.displayText + "</color>\n";
        }

        hudText.text = hasActive ? display.TrimEnd() : "";
    }

    public void SetObjectivesPanelVisible(bool visible)
    {
        if (objectivesPanel != null)
            objectivesPanel.SetActive(visible);
    }
}