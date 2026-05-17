// File: ObjectiveTrigger.cs
// ============================================================================
// Reusable objective trigger. Add as component to any GameObject.
// Set trigger type, action, and objective ID in the Inspector.
// No code changes needed — everything wired in the Inspector.
//
// Trigger Types:
//   OnStart         — fires when the scene loads
//   OnPlayerEnter   — fires when player walks into trigger zone
//   OnCall          — only fires when TriggerObjective() is called externally
//
// Actions:
//   Add             — adds objective to active list
//   Complete        — marks objective as completed
//   AddAndComplete  — adds and immediately completes
//
// Conditions:
//   Optionally require a specific objective to be completed first
//   before this trigger can fire
// ============================================================================
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectiveEntry
{
    public enum ActionType { Add, Complete, AddAndComplete }

    [Tooltip("The objective ID — must match an ID in ObjectiveManager's Definitions array")]
    public string objectiveID;

    [Tooltip("What to do with this objective")]
    public ActionType action = ActionType.Add;
}

public class ObjectiveTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        OnStart,
        OnPlayerEnter,
        OnCall,
    }

    [Header("Trigger Settings")]
    public TriggerType triggerType = TriggerType.OnPlayerEnter;
    public bool oneShot = true;

    [Header("Objectives")]
    public List<ObjectiveEntry> objectives = new List<ObjectiveEntry>();

    [Header("Condition (Optional)")]
    [Tooltip("This trigger will only fire if ALL of these objective IDs are already completed")]
    public List<string> requiredCompletedObjectives = new List<string>();

    [Header("Delay")]
    public float delay = 0f;

    private bool hasFired = false;

    void Start()
    {
        if (triggerType == TriggerType.OnStart)
            FireWithDelay();
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerType != TriggerType.OnPlayerEnter) return;
        if (!other.CompareTag("Player")) return;
        FireWithDelay();
    }

    public void TriggerObjective()
    {
        FireWithDelay();
    }

    bool ConditionsMet()
    {
        if (requiredCompletedObjectives == null || requiredCompletedObjectives.Count == 0)
            return true;

        if (ObjectiveManager.Instance == null) return false;

        foreach (string id in requiredCompletedObjectives)
        {
            if (!ObjectiveManager.Instance.IsComplete(id))
                return false;
        }

        return true;
    }

    void FireWithDelay()
    {
        if (oneShot && hasFired) return;
        if (!ConditionsMet()) return;

        if (delay > 0f)
            StartCoroutine(FireAfterDelay());
        else
            Fire();
    }

    System.Collections.IEnumerator FireAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Fire();
    }

    void Fire()
    {
        if (oneShot && hasFired) return;
        hasFired = true;

        if (ObjectiveManager.Instance == null)
        {
            Debug.LogWarning("[ObjectiveTrigger] ObjectiveManager.Instance is null!");
            return;
        }

        foreach (ObjectiveEntry entry in objectives)
        {
            if (string.IsNullOrEmpty(entry.objectiveID)) continue;

            switch (entry.action)
            {
                case ObjectiveEntry.ActionType.Add:
                    ObjectiveManager.Instance.Add(entry.objectiveID);
                    break;
                case ObjectiveEntry.ActionType.Complete:
                    ObjectiveManager.Instance.Complete(entry.objectiveID);
                    break;
                case ObjectiveEntry.ActionType.AddAndComplete:
                    ObjectiveManager.Instance.AddAndComplete(entry.objectiveID);
                    break;
            }
        }
    }

    public void ResetTrigger()
    {
        hasFired = false;
    }

    void OnDrawGizmos()
    {
        if (triggerType == TriggerType.OnPlayerEnter)
        {
            Gizmos.color = new Color(0f, 1f, 0.5f, 0.2f);
            Gizmos.DrawCube(transform.position, transform.localScale);
            Gizmos.color = new Color(0f, 1f, 0.5f, 0.8f);
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}