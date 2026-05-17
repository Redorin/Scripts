// File: KeySpawner.cs
using UnityEngine;
 
public class KeySpawner : MonoBehaviour
{
    [Header("Key Object")]
    public GameObject keyObject;
 
    [Header("Spawn Settings")]
    public Vector3 spawnOffset = Vector3.zero;
 
    [Header("Dialogue")]
    public string spawnMessage = "Archive access key detected on server desk.";
 
    private bool hasSpawned = false;
 
    void Start()
    {
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
 
        // Fire objective trigger — completes retrieve_archive_key, adds access_archive
        GetComponent<ObjectiveTrigger>()?.TriggerObjective();
 
        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo(spawnMessage);
    }
}
 