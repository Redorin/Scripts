using UnityEngine;
using System.Collections;

public class ResettableObject : MonoBehaviour
{
    [Header("Object Info")]
    public string objectName = "Object";
    public bool canBeReset = true;

    [Header("Reset Settings")]
    public bool resetPosition = true;
    public bool resetRotation = true;
    public bool resetScale = false;
    public bool resetPhysics = true;
    public bool resetActiveState = false;

    [Header("Usage Limits")]
    public bool unlimitedUses = false;
    public int maxResetUses = 1;
    private int currentUses = 0;

    [Header("Reset Type")]
    public ResetType resetType = ResetType.RestoreToOriginal;

    [Header("Toggle State")]
    public bool isInOriginalState = true;

    [Header("Manual Original State (Optional)")]
    public bool useManualOriginalPosition = false;
    public Vector3 manualOriginalPosition;
    public Vector3 manualOriginalRotation;

    [Header("Smooth Restore")]
    public bool smoothRestore = false;
    public float smoothRestoreDuration = 1.0f;

    [Header("Visual Feedback")]
    public bool showResetIndicator = true;
    public Color indicatorColor = Color.cyan;

    [SerializeField] private Vector3 originalLocalPosition;
    [SerializeField] private Quaternion originalLocalRotation;
    [SerializeField] private Vector3 originalLocalScale;
    [SerializeField] private bool originalActiveState;
    [SerializeField] private bool hasStoredOriginalState = false;

    private Vector3 alternateLocalPosition;
    private Quaternion alternateLocalRotation;
    private Vector3 alternateLocalScale;
    private bool alternateActiveState;

    private Rigidbody rb;
    private bool originalKinematic;
    private bool originalGravity;

    public enum ResetType
    {
        RestoreToOriginal,
        Toggle,
        Destroy
    }

    void OnValidate()
    {
        if (!hasStoredOriginalState && !Application.isPlaying)
            StoreOriginalState();
    }

    void Awake()
    {
        if (!hasStoredOriginalState || useManualOriginalPosition)
            StoreOriginalState();

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            originalKinematic = rb.isKinematic;
            originalGravity = rb.useGravity;
        }
    }

    void Start()
    {
        Debug.Log("ResettableObject: " + objectName +
            " | Local pos: " + originalLocalPosition);
    }

    void StoreOriginalState()
    {
        if (useManualOriginalPosition)
        {
            originalLocalPosition = manualOriginalPosition;
            originalLocalRotation = Quaternion.Euler(manualOriginalRotation);
        }
        else
        {
            originalLocalPosition = transform.localPosition;
            originalLocalRotation = transform.localRotation;
        }

        originalLocalScale = transform.localScale;
        originalActiveState = gameObject.activeSelf;
        hasStoredOriginalState = true;
    }

    public bool Reset()
    {
        if (!canBeReset)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(
                    "Object cannot be restored.");
            return false;
        }

        if (!unlimitedUses && currentUses >= maxResetUses)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning(
                    "Correction limit exceeded for this object.");
            return false;
        }

        switch (resetType)
        {
            case ResetType.RestoreToOriginal:
                if (smoothRestore)
                    StartCoroutine(SmoothRestoreToOriginal());
                else
                    RestoreToOriginal();
                break;
            case ResetType.Toggle: ToggleState(); break;
            case ResetType.Destroy: DestroyObject(); break;
        }

        currentUses++;

        if (AdminDialogue.Instance != null)
        {
            if (currentUses == 1)
                AdminDialogue.Instance.AdminInfo("Integrity restored.");
            else if (currentUses >= maxResetUses && !unlimitedUses)
                AdminDialogue.Instance.AdminWarning(
                    "Maximum corrections applied to this object.");
        }

        return true;
    }

    // ── Instant restore ──────────────────────────────────────────────────────
    void RestoreToOriginal()
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider c in allColliders) c.enabled = false;

        Rigidbody[] allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody r in allRigidbodies)
        {
            r.linearVelocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
            r.isKinematic = true;
        }

        if (resetPosition) transform.localPosition = originalLocalPosition;
        if (resetRotation) transform.localRotation = originalLocalRotation;
        if (resetScale) transform.localScale = originalLocalScale;
        if (resetActiveState) gameObject.SetActive(originalActiveState);

        foreach (Collider c in allColliders) c.enabled = true;

        if (resetPhysics && rb != null)
        {
            rb.isKinematic = originalKinematic;
            rb.useGravity = originalGravity;
        }
        else
        {
            foreach (Rigidbody r in allRigidbodies)
            {
                r.isKinematic = false;
                r.useGravity = true;
            }
        }

        NotifyListeners();
    }

    // ── Smooth restore (slides into place) ──────────────────────────────────
    IEnumerator SmoothRestoreToOriginal()
    {
        // Freeze physics immediately
        Rigidbody[] allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody r in allRigidbodies)
        {
            r.linearVelocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
            r.isKinematic = true;
        }

        // Disable colliders during move
        Collider[] allColliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider c in allColliders) c.enabled = false;

        Vector3 startLocalPos = transform.localPosition;
        Quaternion startLocalRot = transform.localRotation;

        float elapsed = 0f;
        while (elapsed < smoothRestoreDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / smoothRestoreDuration);

            if (resetPosition)
                transform.localPosition = Vector3.Lerp(
                    startLocalPos, originalLocalPosition, t);

            if (resetRotation)
                transform.localRotation = Quaternion.Lerp(
                    startLocalRot, originalLocalRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to exact final values
        if (resetPosition) transform.localPosition = originalLocalPosition;
        if (resetRotation) transform.localRotation = originalLocalRotation;
        if (resetScale) transform.localScale = originalLocalScale;
        if (resetActiveState) gameObject.SetActive(originalActiveState);

        // Re-enable colliders
        foreach (Collider c in allColliders) c.enabled = true;

        // Restore physics
        if (resetPhysics && rb != null)
        {
            rb.isKinematic = originalKinematic;
            rb.useGravity = originalGravity;
        }
        else
        {
            foreach (Rigidbody r in allRigidbodies)
            {
                r.isKinematic = false;
                r.useGravity = true;
            }
        }

        NotifyListeners();
    }

    void NotifyListeners()
    {
        ConduitResettable conduit = GetComponentInParent<ConduitResettable>();
        if (conduit != null) conduit.OnConduitReset();

        BurntCableResettable burntCable =
            GetComponentInParent<BurntCableResettable>();
        if (burntCable != null) burntCable.OnCableRestored();
    }

    void ToggleState()
    {
        if (isInOriginalState)
        {
            if (resetPosition) transform.localPosition = alternateLocalPosition;
            if (resetRotation) transform.localRotation = alternateLocalRotation;
            if (resetScale) transform.localScale = alternateLocalScale;
            if (resetActiveState) gameObject.SetActive(alternateActiveState);
            isInOriginalState = false;
        }
        else
        {
            RestoreToOriginal();
            isInOriginalState = true;
        }
    }

    void DestroyObject()
    {
        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminWarning(
                "Object removed from environment.");
        Destroy(gameObject);
    }

    public void SetAlternateState(Vector3 localPos, Quaternion localRot,
        Vector3 scale, bool active)
    {
        alternateLocalPosition = localPos;
        alternateLocalRotation = localRot;
        alternateLocalScale = scale;
        alternateActiveState = active;
    }

    public Vector3 GetOriginalPosition()
    {
        if (transform.parent != null)
            return transform.parent.TransformPoint(originalLocalPosition);
        return originalLocalPosition;
    }

    public Quaternion GetOriginalRotation()
    {
        if (transform.parent != null)
            return transform.parent.rotation * originalLocalRotation;
        return originalLocalRotation;
    }

    public void ForceRestore()
    {
        Collider[] allColliders = GetComponentsInChildren<Collider>(true);
        foreach (Collider c in allColliders) c.enabled = false;

        Rigidbody[] allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody r in allRigidbodies)
        {
            r.linearVelocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
            r.isKinematic = true;
        }

        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;

        foreach (Collider c in allColliders) c.enabled = true;
        foreach (Rigidbody r in allRigidbodies)
        {
            r.isKinematic = false;
            r.useGravity = true;
        }

        Debug.Log(objectName + " force restored.");
    }

    public bool CanBeReset()
    {
        if (!canBeReset) return false;
        if (!unlimitedUses && currentUses >= maxResetUses) return false;
        return true;
    }

    public int GetRemainingUses() =>
        unlimitedUses ? 999 : maxResetUses - currentUses;

    void OnDrawGizmos()
    {
        if (showResetIndicator)
        {
            Gizmos.color = canBeReset ? indicatorColor : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}