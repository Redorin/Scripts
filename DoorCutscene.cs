using UnityEngine;
using System.Collections;

public class DoorCutscene : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public Transform doorKnob;
    public CeilingCollapseOnDoor ceilingCollapse;
    public InteractableDoor door;
    public PlayerMovement playerMovement;
    public MouseMovement mouseMovement;
    public PlayerInteraction playerInteraction;

    [Header("Camera Movement")]
    public float lookAtKnobDuration = 0.8f;
    public float watchDebrisDuration = 1.5f;
    public float returnToNormalDuration = 0.8f;

    [Header("Timing")]
    public float delayBeforeDoorOpens = 0.5f;
    public float delayBeforeDebris = 0.3f;

    [Header("State")]
    public bool hasPlayed = false;

    private bool isPlaying = false;
    private Quaternion originalCameraRotation;
    private float originalXRotation;

    public void TriggerCutscene()
    {
        if (hasPlayed || isPlaying) return;
        StartCoroutine(PlayDoorCutscene());
    }

    IEnumerator PlayDoorCutscene()
    {
        isPlaying = true;
        hasPlayed = true;

        // Step 1 — Disable player control
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (mouseMovement != null)
            mouseMovement.enabled = false;
        if (playerInteraction != null)
            playerInteraction.enabled = false;

        // Store original camera rotation
        originalCameraRotation = playerCamera.rotation;
        if (mouseMovement != null)
            originalXRotation = mouseMovement.GetXRotation();

        // Step 2 — Look at door knob smoothly
        yield return StartCoroutine(LookAt(doorKnob.position, lookAtKnobDuration));

        // Step 3 — Short pause then open door
        yield return new WaitForSeconds(delayBeforeDoorOpens);

        if (door != null)
            door.Interact();

        // Step 4 — Wait then trigger debris
        yield return new WaitForSeconds(delayBeforeDebris);

        if (ceilingCollapse != null)
            ceilingCollapse.TriggerCollapse();

        // Step 5 — Watch debris fall
        yield return new WaitForSeconds(watchDebrisDuration);

        // Step 6 — Return camera to original rotation
        yield return StartCoroutine(ReturnCamera(returnToNormalDuration));

        // Step 7 — Re-enable player control
        EnablePlayer();
    }

    IEnumerator LookAt(Vector3 targetPosition, float duration)
    {
        Quaternion startRot = playerCamera.rotation;
        Vector3 direction = targetPosition - playerCamera.position;
        Quaternion endRot = Quaternion.LookRotation(direction);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            playerCamera.rotation = Quaternion.Lerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.rotation = endRot;
    }

    IEnumerator ReturnCamera(float duration)
    {
        Quaternion startRot = playerCamera.rotation;
        Quaternion endRot = originalCameraRotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            playerCamera.rotation = Quaternion.Lerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.rotation = endRot;
    }

    void EnablePlayer()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (mouseMovement != null)
        {
            mouseMovement.enabled = true;
            mouseMovement.SetXRotation(originalXRotation);
        }
        if (playerInteraction != null)
            playerInteraction.enabled = true;

        isPlaying = false;
        Debug.Log("[DoorCutscene] Complete.");
    }
}