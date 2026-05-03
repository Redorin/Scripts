using UnityEngine;

public class AdminStuckHint : MonoBehaviour
{
    [Header("Settings")]
    public float timeBeforeHint = 120f;
    public bool triggerOnce = true;

    [Header("Hint Lines")]
    [TextArea(2, 4)]
    public string[] hintLines;

    private float timer = 0f;
    private bool triggered = false;
    private bool playerInZone = false;

    void Update()
    {
        if (!playerInZone || triggered) return;

        timer += Time.deltaTime;
        if (timer >= timeBeforeHint)
            TriggerHint();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            timer = 0f;
        }
    }

    void TriggerHint()
    {
        if (AdminDialogue.Instance == null) return;

        foreach (string line in hintLines)
            AdminDialogue.Instance.AdminWarning(line);

        if (triggerOnce)
            triggered = true;
    }

    public void ResetHint()
    {
        triggered = false;
        timer = 0f;
    }
}