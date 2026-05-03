using UnityEngine;
using System.Collections;

// Attach to the AdminAccessDoor in Chapter 3.
// When player presses E, transitions to Chapter 4.

public class Chapter3To4Transition : MonoBehaviour
{
    [Header("Chapter Roots")]
    public GameObject chapter3Root;
    public GameObject chapter4Root;

    [Header("Spawn Point")]
    public Transform chapter4SpawnPoint;

    [Header("Dialogue")]
    public string[] transitionDialogue = {
        "ADMIN ACCESS granted.",
        "Entering fragmented sector.",
        "Observer alignment: pending."
    };

    private bool hasTriggered = false;

    // Called by PlayerInteraction when player presses E on this door
    public void Interact()
    {
        if (hasTriggered) return;
        hasTriggered = true;
        StartCoroutine(TransitionSequence());
    }

    IEnumerator TransitionSequence()
    {
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in transitionDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        yield return new WaitForSeconds(3f);

        // Teleport player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && chapter4SpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = chapter4SpawnPoint.position;
            player.transform.rotation = chapter4SpawnPoint.rotation;
            if (cc != null) cc.enabled = true;
        }

        // Restore player body (Chapter 3 hid it)
        // Chapter4Manager will manage body visibility
        GameObject body = GameObject.Find("Body");
        if (body != null) body.SetActive(true);

        // Swap chapters
        if (chapter3Root != null) chapter3Root.SetActive(false);
        if (chapter4Root != null) chapter4Root.SetActive(true);

        // Start Chapter 4
        if (Chapter4Manager.Instance != null)
            Chapter4Manager.Instance.StartChapter();
    }
}