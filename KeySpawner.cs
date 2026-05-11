using UnityEngine;

// Attach to an empty GameObject near the server desk.
// Call SpawnKey() to make the archive key appear after puzzle is solved.
// The key GameObject should be a HoldableItem, placed in scene but inactive at start.

public class KeySpawner : MonoBehaviour
{
    [Header("Key Object")]
    // Place the key HoldableItem in the scene, set inactive in editor
    public GameObject keyObject;

    [Header("Spawn Settings")]
    public Vector3 spawnOffset = Vector3.zero;  // offset from this transform

    [Header("Dialogue")]
    public string spawnMessage = "Archive access key detected on server desk.";

    private bool hasSpawned = false;

    void Start()
    {
        // Make sure key starts hidden
        if (keyObject != null)
            keyObject.SetActive(false);
    }

    public void SpawnKey()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        if (keyObject != null)
        {
            keyObject.transform.position = transform.position + spawnOffset;
            keyObject.SetActive(true);
        }

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(spawnMessage);
    }
}