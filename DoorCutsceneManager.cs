using UnityEngine;
using System.Collections;

public class DoorCutsceneManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public Transform playerBody;
    public Transform doorKnob;
    public Transform doorTarget;
    public Transform resetItemTransform;
    public InteractableDoor door;
    public CeilingCollapseOnDoor ceilingCollapse;
    public PlayerMovement playerMovement;
    public MouseMovement mouseMovement;
    public PlayerInteraction playerInteraction;
    public CharacterController characterController;

    [Header("Player Positioning")]
    public Transform playerStartPosition;
    public float repositionDuration = 0.5f;

    [Header("State")]
    public bool hasPlayed = false;

    [Header("-- STEP 1: Look At Door Handle --")]
    public float lookAtHandleDuration = 0.8f;
    public Vector3 lookAtHandleAngle = new Vector3(10f, 0f, 0f);

    [Header("-- STEP 2: Step Forward --")]
    public float stepForwardDistance = 0.6f;
    public float stepForwardDuration = 0.8f;
    public float stepDelay = 0.3f;

    [Header("-- STEP 3: Look Up Before Debris --")]
    public float lookUpDuration = 0.5f;
    public Vector3 lookUpAngle = new Vector3(-20f, 0f, 0f);

    [Header("-- STEP 4: Debris Falls --")]
    public float debrisWaitBeforeLook = 0.3f;

    [Header("-- STEP 5: Look Away In Shock --")]
    public float lookAwayDuration = 0.3f;
    public Vector3 lookAwayAngle = new Vector3(5f, -40f, 0f);
    public float holdShockDuration = 0.4f;

    [Header("-- STEP 6: Look Forward Downward --")]
    public float lookAtMessDuration = 0.8f;
    public Vector3 lookAtMessAngle = new Vector3(15f, 0f, 0f);
    public float observeMessDuration = 1.2f;

    [Header("-- STEP 7: Look At Reset Item --")]
    public float lookAtResetDuration = 0.8f;
    public float holdOnResetDuration = 1f;

    [Header("-- STEP 8: End --")]
    public float endHoldDuration = 0.5f;

    private bool isPlaying = false;

    public void TriggerCutscene()
    {
        if (hasPlayed || isPlaying) return;
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        isPlaying = true;
        hasPlayed = true;

        // Disable player
        DisablePlayer();

        // ── STEP 0: Reposition player in front of door ──
        if (playerStartPosition != null)
            yield return StartCoroutine(RepositionPlayer(
                playerStartPosition.position,
                playerStartPosition.rotation,
                repositionDuration));

        // Small settle delay
        yield return new WaitForSeconds(0.1f);

        // ── STEP 1: Look at door handle ──
        yield return StartCoroutine(
            RotateCamera(lookAtHandleAngle, lookAtHandleDuration));

        // ── STEP 2: Open door then step forward ──
        if (door != null)
            door.Interact();

        yield return new WaitForSeconds(stepDelay);

        yield return StartCoroutine(
            StepForward(stepForwardDistance, stepForwardDuration));

        // ── STEP 3: Look up slightly ──
        yield return StartCoroutine(
            RotateCamera(lookUpAngle, lookUpDuration));

        // ── STEP 4: Trigger debris ──
        yield return new WaitForSeconds(debrisWaitBeforeLook);

        if (ceilingCollapse != null)
            ceilingCollapse.TriggerCollapse();

        // ── STEP 5: Look away in shock ──
        yield return StartCoroutine(
            RotateCamera(lookAwayAngle, lookAwayDuration));

        yield return new WaitForSeconds(holdShockDuration);

        // ── STEP 6: Look forward and slightly down at mess ──
        yield return StartCoroutine(
            RotateCamera(lookAtMessAngle, lookAtMessDuration));

        yield return new WaitForSeconds(observeMessDuration);

        // ── STEP 7: Look at Reset Item ──
        if (resetItemTransform != null)
        {
            yield return StartCoroutine(
                LookAtTarget(resetItemTransform.position, lookAtResetDuration));
        }

        yield return new WaitForSeconds(holdOnResetDuration);

        // ── STEP 8: End cutscene ──
        yield return new WaitForSeconds(endHoldDuration);

        EnablePlayer();
        isPlaying = false;
    }

    IEnumerator RepositionPlayer(Vector3 targetPosition,
        Quaternion targetRotation, float duration)
    {
        Vector3 startPos = playerBody.position;
        Quaternion startBodyRot = playerBody.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            if (characterController != null)
            {
                characterController.enabled = false;
                playerBody.position = Vector3.Lerp(startPos, targetPosition, t);
                playerBody.rotation = Quaternion.Lerp(startBodyRot, targetRotation, t);
                characterController.enabled = true;
            }
            else
            {
                playerBody.position = Vector3.Lerp(startPos, targetPosition, t);
                playerBody.rotation = Quaternion.Lerp(startBodyRot, targetRotation, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to exact final values
        if (characterController != null)
        {
            characterController.enabled = false;
            playerBody.position = targetPosition;
            playerBody.rotation = targetRotation;
            characterController.enabled = true;
        }
        else
        {
            playerBody.position = targetPosition;
            playerBody.rotation = targetRotation;
        }

        // Sync MouseMovement to new body rotation
        if (mouseMovement != null)
        {
            float currentX = playerCamera.localEulerAngles.x;
            if (currentX > 180f) currentX -= 360f;
            mouseMovement.SetXRotation(currentX);

            Vector3 euler = targetRotation.eulerAngles;
            mouseMovement.SetBodyYRotation(euler.y);
        }

        // Smoothly look at door face after repositioning
        Vector3 lookTarget = doorTarget != null
            ? doorTarget.position
            : door.transform.position;

        yield return StartCoroutine(LookAtTargetSmooth(lookTarget, 0.3f));
    }

    IEnumerator RotateCamera(Vector3 targetLocalEuler, float duration)
    {
        Quaternion startRot = playerCamera.localRotation;
        Quaternion endRot = Quaternion.Euler(targetLocalEuler);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            playerCamera.localRotation = Quaternion.Lerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerCamera.localRotation = endRot;
    }

    IEnumerator LookAtTarget(Vector3 targetWorldPosition, float duration)
    {
        Quaternion startRot = playerCamera.rotation;
        Vector3 direction = (targetWorldPosition - playerCamera.position).normalized;
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

    IEnumerator LookAtTargetSmooth(Vector3 targetWorldPosition, float duration)
    {
        Quaternion startRot = playerCamera.localRotation;
        Vector3 direction = (targetWorldPosition - playerCamera.position).normalized;
        Quaternion worldEnd = Quaternion.LookRotation(direction);

        // Convert world rotation to local
        Quaternion parentRot = playerCamera.parent != null
            ? playerCamera.parent.rotation
            : Quaternion.identity;
        Quaternion localEnd = Quaternion.Inverse(parentRot) * worldEnd;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            playerCamera.localRotation = Quaternion.Lerp(startRot, localEnd, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.localRotation = localEnd;

        // Sync X rotation to MouseMovement
        float finalX = playerCamera.localEulerAngles.x;
        if (finalX > 180f) finalX -= 360f;
        if (mouseMovement != null)
            mouseMovement.SetXRotation(finalX);
    }

    IEnumerator StepForward(float distance, float duration)
    {
        // Disable door colliders so player doesn't get blocked
        Collider[] doorColliders = GetComponentsInChildren<Collider>();
        foreach (Collider dc in doorColliders)
            dc.enabled = false;

        float elapsed = 0f;
        float speed = distance / duration;

        while (elapsed < duration)
        {
            float step = speed * Time.deltaTime;

            if (characterController != null)
                characterController.Move(playerBody.forward * step);
            else
                playerBody.position += playerBody.forward * step;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Re-enable door colliders
        foreach (Collider dc in doorColliders)
            dc.enabled = true;
    }

    void DisablePlayer()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (mouseMovement != null)
            mouseMovement.enabled = false;
        if (playerInteraction != null)
            playerInteraction.enabled = false;
    }

    void EnablePlayer()
    {
        // Get final camera X and convert to -180/180 range
        float finalX = playerCamera.localEulerAngles.x;
        if (finalX > 180f) finalX -= 360f;
        finalX = Mathf.Clamp(finalX, -89f, 89f);

        // Zero out Y and Z to prevent sideways drift
        playerCamera.localRotation = Quaternion.Euler(finalX, 0f, 0f);

        if (mouseMovement != null)
        {
            mouseMovement.enabled = true;
            mouseMovement.SetXRotation(finalX);
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerInteraction != null)
            playerInteraction.enabled = true;

        Debug.Log("[DoorCutscene] Complete. Player enabled.");
    }
}