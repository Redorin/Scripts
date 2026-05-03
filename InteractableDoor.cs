using UnityEngine;
using System.Collections;

public class InteractableDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Auto Close")]
    public bool autoClose = true;
    public float autoCloseDelay = 3f;

    [Header("Door Type")]
    public bool slidesDoor = false;
    public Vector3 slideDirection = Vector3.right;
    public float slideDistance = 2f;

    [Header("Knob Animation")]
    public Animator knobAnimator;
    public string knobTriggerName = "TurnKnob";

    [Header("Handle Animation")]
    public DoorHandleAnimator handleAnimator;

    private bool isOpen = false;
    private bool isMoving = false;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private Quaternion closedRotation;
    private Vector3 closedPosition;
    private Coroutine autoCloseCoroutine;

    void Start()
    {
        closedRotation = transform.rotation;
        closedPosition = transform.position;

        if (slidesDoor)
            targetPosition = closedPosition;
        else
            targetRotation = closedRotation;
    }

    void Update()
    {
        if (slidesDoor)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * openSpeed);

            if (isMoving && Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                OnMovementComplete();
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);

            if (isMoving && Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                transform.rotation = targetRotation;
                OnMovementComplete();
            }
        }
    }

    public void Interact()
    {
        // Prevent interaction while door is moving
        if (isMoving) return;

        isMoving = true;
        isOpen = !isOpen;

        // Play handle animation once
        if (handleAnimator != null)
            handleAnimator.PlayPressAnimation();

        // Play knob animation
        if (knobAnimator != null)
            knobAnimator.SetTrigger(knobTriggerName);

        // Stop existing auto-close
        if (autoCloseCoroutine != null)
            StopCoroutine(autoCloseCoroutine);

        StartCoroutine(OpenDoorAfterDelay(0.2f));
    }

    IEnumerator OpenDoorAfterDelay(float delay)
    {
        SetPlayerCollision(false);

        yield return new WaitForSeconds(delay);

        if (slidesDoor)
        {
            targetPosition = isOpen
                ? closedPosition + slideDirection.normalized * slideDistance
                : closedPosition;
        }
        else
        {
            targetRotation = isOpen
                ? closedRotation * Quaternion.Euler(0, openAngle, 0)
                : closedRotation;
        }

        if (isOpen && autoClose)
            autoCloseCoroutine = StartCoroutine(AutoCloseAfterDelay());
    }

    IEnumerator AutoCloseAfterDelay()
    {
        yield return new WaitForSeconds(autoCloseDelay);

        if (isOpen && !isMoving)
            Interact();
    }

    public void ForceClose()
    {
        isOpen = false;
        isMoving = false;

        if (slidesDoor)
        {
            targetPosition = closedPosition;
            transform.position = closedPosition;
        }
        else
        {
            targetRotation = closedRotation;
            transform.rotation = closedRotation;
        }

        SetPlayerCollision(true);
    }

    void OnMovementComplete()
    {
        isMoving = false;
        SetPlayerCollision(true);
    }

    void SetPlayerCollision(bool enable)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
        Collider[] doorColliders = GetComponentsInChildren<Collider>();

        foreach (Collider pc in playerColliders)
        {
            foreach (Collider dc in doorColliders)
            {
                if (pc != null && dc != null)
                    Physics.IgnoreCollision(pc, dc, !enable);
            }
        }
    }

    public bool IsOpen() => isOpen;
    public bool IsMoving() => isMoving;
}