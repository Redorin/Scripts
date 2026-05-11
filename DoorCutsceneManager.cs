using UnityEngine;
using System.Collections;

public class DoorCutsceneManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;
    public Transform playerBody;
    public Transform playerRoot;
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

    [Header("-- STEP 2: Step Forward --")]
    public float stepForwardDistance = 0.6f;
    public float stepForwardDuration = 0.8f;
    public float stepDelay = 0.3f;

    [Header("-- STEP 3: Look Up Before Debris --")]
    public float lookUpDuration = 1.0f;
    public Vector3 lookUpAngle = new Vector3(-30f, 0f, 0f);

    [Header("-- STEP 4: Debris Falls --")]
    public float debrisWaitBeforeLook = 0.2f;

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

    [Header("Controls UI")]
    public ControlsUI controlsUI;

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

        DisablePlayer();

        // ── STEP 0: Reposition player in front of door ──
        if (playerStartPosition != null)
            yield return StartCoroutine(RepositionPlayer(
                playerStartPosition.position,
                playerStartPosition.rotation,
                repositionDuration));

        yield return new WaitForSeconds(0.1f);

        // ── STEP 1: Look at door handle in world space ──
        yield return StartCoroutine(
            LookAtTarget(doorKnob.position, lookAtHandleDuration));

        // ── STEP 2: Open door + look forward + step simultaneously ──
        if (door != null)
            door.Interact();

        yield return new WaitForSeconds(stepDelay);

        // Look forward and step at the same time
        StartCoroutine(RotateCamera(Vector3.zero, stepForwardDuration));
        yield return StartCoroutine(
            StepForward(stepForwardDistance, stepForwardDuration));

        // ── STEP 3 + 4: Look up AND trigger debris simultaneously ──
        StartCoroutine(RotateCamera(lookUpAngle, lookUpDuration));

        yield return new WaitForSeconds(debrisWaitBeforeLook);

        if (ceilingCollapse != null)
            ceilingCollapse.TriggerCollapse();

        yield return new WaitForSeconds(lookUpDuration - debrisWaitBeforeLook);

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
            yield return StartCoroutine(
                LookAtTarget(resetItemTransform.position, lookAtResetDuration));

        yield return new WaitForSeconds(holdOnResetDuration);

        // ── STEP 8: End cutscene ──
        yield return new WaitForSeconds(endHoldDuration);

        EnablePlayer();
        isPlaying = false;
    }

    IEnumerator RepositionPlayer(Vector3 targetPosition,
        Quaternion targetRotation, float duration)
    {
        Vector3 startPos = GetPlayerRootTransform().position;
        Quaternion startRot = GetPlayerRootTransform().rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            if (characterController != null)
                characterController.enabled = false;

            GetPlayerRootTransform().position =
                Vector3.Lerp(startPos, targetPosition, t);
            GetPlayerRootTransform().rotation =
                Quaternion.Lerp(startRot, targetRotation, t);

            if (characterController != null)
                characterController.enabled = true;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to exact values
        if (characterController != null)
            characterController.enabled = false;
        GetPlayerRootTransform().position = targetPosition;
        GetPlayerRootTransform().rotation = targetRotation;
        if (characterController != null)
            characterController.enabled = true;

        // Sync MouseMovement
        SyncMouseMovementToCurrentTransforms();

        // ── FIX 1: Reset camera to straight forward, no tilt ──
        playerCamera.localRotation = Quaternion.identity;
        if (mouseMovement != null)
            mouseMovement.SetXRotation(0f);
    }

    Transform GetPlayerRootTransform()
    {
        if (playerRoot != null) return playerRoot;
        if (playerBody != null && playerBody.parent != null)
            return playerBody.parent;
        return playerBody;
    }

    void SyncMouseMovementToCurrentTransforms()
    {
        if (mouseMovement == null) return;

        float currentX = playerCamera.localEulerAngles.x;
        if (currentX > 180f) currentX -= 360f;
        mouseMovement.SetXRotation(currentX);

        float currentY = GetPlayerRootTransform().eulerAngles.y;
        mouseMovement.SetBodyYRotation(currentY);
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
        Vector3 direction =
            (targetWorldPosition - playerCamera.position).normalized;
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

        // Sync X after world space look
        float finalX = playerCamera.localEulerAngles.x;
        if (finalX > 180f) finalX -= 360f;
        if (mouseMovement != null)
            mouseMovement.SetXRotation(finalX);
    }

    IEnumerator StepForward(float distance, float duration)
    {
        Collider[] doorColliders = GetComponentsInChildren<Collider>();
        foreach (Collider dc in doorColliders)
            dc.enabled = false;

        float elapsed = 0f;
        float speed = distance / duration;

        while (elapsed < duration)
        {
            float step = speed * Time.deltaTime;

            if (characterController != null)
                characterController.Move(
                    GetPlayerRootTransform().forward * step);
            else
                GetPlayerRootTransform().position +=
                    GetPlayerRootTransform().forward * step;

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (Collider dc in doorColliders)
            dc.enabled = true;
    }

    void DisablePlayer()
    {
        if (playerMovement != null) playerMovement.enabled = false;
        if (mouseMovement != null) mouseMovement.enabled = false;
        if (playerInteraction != null) playerInteraction.enabled = false;

        if (playerInteraction != null)
    {
        playerInteraction.HideUI();
    }

    }

    void EnablePlayer()
    {
        Vector3 worldEuler = playerCamera.rotation.eulerAngles;

        float targetY = worldEuler.y;
        mouseMovement.SetBodyYRotation(targetY);
        GetPlayerRootTransform().eulerAngles = new Vector3(0f, targetY, 0f);

        float targetX = worldEuler.x;
        if (targetX > 180f) targetX -= 360f;
        targetX = Mathf.Clamp(targetX, -89f, 89f);

        playerCamera.localRotation = Quaternion.Euler(targetX, 0f, 0f);
        mouseMovement.SetXRotation(targetX);

        if (mouseMovement != null) mouseMovement.enabled = true;
        if (playerMovement != null) playerMovement.enabled = true;
        if (playerInteraction != null) playerInteraction.enabled = true;

        if (controlsUI != null)
            controlsUI.ShowItemControls();

            if (playerInteraction != null)
        playerInteraction.RestoreUI();

        Debug.Log("[DoorCutscene] Complete. Player Y=" + targetY +
            " Camera X=" + targetX);
    }
}