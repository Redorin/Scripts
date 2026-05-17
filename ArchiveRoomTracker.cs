// File: ArchiveRoomTracker.cs
using UnityEngine;

public class ArchiveRoomTracker : MonoBehaviour
{
    public static ArchiveRoomTracker Instance { get; private set; }

    public bool hasVisited = false;

    [Header("Dialogue")]
    public string[] firstVisitDialogue = {
        "Archive room — access logged.",
        "Session records indicate prior use of this terminal.",
        "Interesting."
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasVisited) return;
        if (!other.CompareTag("Player")) return;

        hasVisited = true;

        GetComponent<ObjectiveTrigger>()?.TriggerObjective();

        if (AdminDialogue.Instance != null)
            foreach (string line in firstVisitDialogue)
                AdminDialogue.Instance.AdminInfo(line);
    }
}