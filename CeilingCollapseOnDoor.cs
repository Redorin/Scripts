using UnityEngine;
using System.Collections;

public class CeilingCollapseOnDoor : MonoBehaviour
{
    [Header("Debris")]
    public GameObject[] debrisObjects;
    public float fallDelay = 0.5f;

    [Header("Reset Device Drop")]
    public GameObject resetDeviceObject;
    public bool dropResetDevice = true;

    [Header("Camera Shake")]
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;

    [Header("Dialogue")]
    public string[] collapseDialogue = {
        "Structural failure detected.",
        "Rollback device granted.",
        "Limited use. Use responsibly."
    };

    [Header("State")]
    public bool hasCollapsed = false;

    private Camera playerCamera;
    private Vector3 originalCameraPos;
    private InteractableDoor door;

    void Start()
    {
        playerCamera = Camera.main;
        door = GetComponent<InteractableDoor>();

        foreach (GameObject debris in debrisObjects)
        {
            if (debris == null) continue;
            Rigidbody rb = debris.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }

        if (resetDeviceObject != null)
        {
            Rigidbody rb = resetDeviceObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }
    }

    // Called by PlayerInteraction when no cutscene exists
    public void InteractAndCollapse()
    {
        if (door != null)
            door.Interact();

        TriggerCollapse();
    }

    // Called by DoorCutsceneManager via Timeline signal
    public void TriggerCollapse()
    {
        if (hasCollapsed) return;
        hasCollapsed = true;
        StartCoroutine(CollapseSequence());
    }

    IEnumerator CollapseSequence()
    {
        yield return new WaitForSeconds(fallDelay);

        foreach (GameObject debris in debrisObjects)
        {
            if (debris == null) continue;
            Rigidbody rb = debris.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }

        if (dropResetDevice && resetDeviceObject != null)
        {
            yield return new WaitForSeconds(0.2f);
            Rigidbody rb = resetDeviceObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }

        StartCoroutine(ShakeCamera());

        yield return new WaitForSeconds(1f);

        if (AdminDialogue.Instance != null)
        {
            foreach (string line in collapseDialogue)
                AdminDialogue.Instance.AdminWarning(line);
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
            playerCamera.transform.localPosition = originalCameraPos +
                new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.localPosition = originalCameraPos;
    }
}