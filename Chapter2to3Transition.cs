using UnityEngine;
using System.Collections;

// Attach to the HiddenDoor in Chapter 2's ObjectRoom.
// When player walks through, disables Chapter 2 and enables Chapter 3.

public class Chapter2To3Transition : MonoBehaviour
{
    [Header("Chapter Roots")]
    public GameObject chapter2Root;
    public GameObject chapter3Root;

    [Header("Spawn Point")]
    public Transform chapter3SpawnPoint;    // Empty GO at start of Chapter 3 hallway

    [Header("Dialogue")]
    public string[] transitionDialogue = {
        "Sector boundary crossed.",
        "Returning to initialization point.",
        "Warning: environment integrity compromised."
    };

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        StartCoroutine(TransitionSequence());
    }

    IEnumerator TransitionSequence()
    {
        // Play dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in transitionDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        yield return new WaitForSeconds(2f);

        // Teleport player to Chapter 3 start
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && chapter3SpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = chapter3SpawnPoint.position;
            player.transform.rotation = chapter3SpawnPoint.rotation;
            if (cc != null) cc.enabled = true;
        }

        // Swap chapters
        if (chapter2Root != null) chapter2Root.SetActive(false);
        if (chapter3Root != null) chapter3Root.SetActive(true);

        // Start Chapter 3 sequence
        if (Chapter3Manager.Instance != null)
            Chapter3Manager.Instance.StartChapter();
    }
}