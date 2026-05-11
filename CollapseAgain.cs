using UnityEngine;
using System.Collections;

// Attach to an invisible trigger zone PAST the first collapsed ceiling area.
// When player walks through, a second set of debris falls and cannot be reset.
// Shelves in the hallway also fall simultaneously to block the path.

public class CollapseAgain : MonoBehaviour
{
    [Header("Debris")]
    public GameObject[] debrisObjects;
    public float fallDelay = 0.5f;

    [Header("Shelves")]
    public GameObject[] shelfObjects;       // Shelf GameObjects to fall with debris
    public float shelfFallDelay = 0f;       // Extra delay after debris (0 = same time)

    [Header("Camera Shake")]
    public float shakeDuration = 0.4f;
    public float shakeMagnitude = 0.15f;

    [Header("Dialogue")]
    public string[] collapseDialogue = {
        "Structural failure detected.",
        "Rollback limit exceeded for this object.",
        "Find another way."
    };

    private bool hasTriggered = false;
    private Camera playerCamera;
    private Vector3 originalCameraPos;

    void Start()
    {
        playerCamera = Camera.main;

        // Keep debris frozen at ceiling at start
        foreach (GameObject debris in debrisObjects)
        {
            if (debris == null) continue;
            Rigidbody rb = debris.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            ResettableObject resettable = debris.GetComponent<ResettableObject>();
            if (resettable != null) resettable.canBeReset = false;
        }

        // Keep shelves frozen at start
        foreach (GameObject shelf in shelfObjects)
        {
            if (shelf == null) continue;
            Rigidbody rb = shelf.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            // Shelves can't be reset either — path is permanently blocked
            ResettableObject resettable = shelf.GetComponent<ResettableObject>();
            if (resettable != null) resettable.canBeReset = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        StartCoroutine(CollapseSequence());
    }

    IEnumerator CollapseSequence()
    {
        yield return new WaitForSeconds(fallDelay);

        // Drop ceiling debris
        foreach (GameObject debris in debrisObjects)
        {
            if (debris == null) continue;
            Rigidbody rb = debris.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }

        // Drop shelves (with optional extra delay)
        StartCoroutine(DropShelves());

        // Camera shake
        if (playerCamera != null)
            StartCoroutine(ShakeCamera());

        yield return new WaitForSeconds(1f);

        // Admin dialogue
        if (AdminDialogue.Instance != null)
        {
            foreach (string line in collapseDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }
    }

    IEnumerator DropShelves()
    {
        if (shelfFallDelay > 0f)
            yield return new WaitForSeconds(shelfFallDelay);

        foreach (GameObject shelf in shelfObjects)
        {
            if (shelf == null) continue;
            Rigidbody rb = shelf.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }
    }

    IEnumerator ShakeCamera()
    {
        if (playerCamera == null) yield break;

        originalCameraPos = playerCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            playerCamera.transform.localPosition = originalCameraPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.localPosition = originalCameraPos;
    }
}