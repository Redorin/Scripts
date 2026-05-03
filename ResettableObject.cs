using UnityEngine;

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
    public Vector3 manualOriginalPosition;      // LOCAL position
    public Vector3 manualOriginalRotation;

    [Header("Visual Feedback")]
    public bool showResetIndicator = true;
    public Color indicatorColor = Color.cyan;

    // Stored in LOCAL space to avoid parent offset issues
    [SerializeField] private Vector3 originalLocalPosition;
    [SerializeField] private Quaternion originalLocalRotation;
    [SerializeField] private Vector3 originalLocalScale;
    [SerializeField] private bool originalActiveState;
    [SerializeField] private bool hasStoredOriginalState = false;

    // Alternate state for toggles
    private Vector3 alternateLocalPosition;
    private Quaternion alternateLocalRotation;
    private Vector3 alternateLocalScale;
    private bool alternateActiveState;

    // Physics
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
        if (!hasStoredOriginalState)
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
        Debug.Log("ResettableObject: " + objectName + " | Local pos: " + originalLocalPosition);
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
            // Store LOCAL position - not affected by parent offset
            originalLocalPosition = transform.localPosition;
            originalLocalRotation = transform.localRotation;
        }

        originalLocalScale = transform.localScale;
        originalActiveState = gameObject.activeSelf;
        hasStoredOriginalState = true;

        Debug.Log("Stored LOCAL state for: " + objectName + " at local pos: " + originalLocalPosition);
    }

    public bool Reset()
    {
        if (!canBeReset)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning("Object cannot be restored.");
            return false;
        }

        if (!unlimitedUses && currentUses >= maxResetUses)
        {
            if (AdminDialogue.Instance != null)
                AdminDialogue.Instance.AdminWarning("Correction limit exceeded for this object.");
            return false;
        }

        switch (resetType)
        {
            case ResetType.RestoreToOriginal: RestoreToOriginal(); break;
            case ResetType.Toggle: ToggleState(); break;
            case ResetType.Destroy: DestroyObject(); break;
        }

        currentUses++;

        if (AdminDialogue.Instance != null)
        {
            if (currentUses == 1)
                AdminDialogue.Instance.AdminInfo("Integrity restored.");
            else if (currentUses >= maxResetUses && !unlimitedUses)
                AdminDialogue.Instance.AdminWarning("Maximum corrections applied to this object.");
        }

        return true;
    }

    void RestoreToOriginal()
    {
        // Stop physics first
        if (resetPhysics && rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = originalKinematic;
            rb.useGravity = originalGravity;
        }

        // Restore LOCAL position
        if (resetPosition)
            transform.localPosition = originalLocalPosition;

        if (resetRotation)
            transform.localRotation = originalLocalRotation;

        if (resetScale)
            transform.localScale = originalLocalScale;

        if (resetActiveState)
            gameObject.SetActive(originalActiveState);
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
            AdminDialogue.Instance.AdminWarning("Object removed from environment.");
        Destroy(gameObject);
    }

    public void SetAlternateState(Vector3 localPos, Quaternion localRot, Vector3 scale, bool active)
    {
        alternateLocalPosition = localPos;
        alternateLocalRotation = localRot;
        alternateLocalScale = scale;
        alternateActiveState = active;
    }

    // Public accessors for DebrisGroup animation
    public Vector3 GetOriginalPosition()
    {
        // Returns WORLD position for animation purposes
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

    // Called by DebrisGroup - bypasses all checks
    public void ForceRestore()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Restore local position
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;

        Debug.Log(objectName + " force restored to local pos: " + originalLocalPosition);
    }

    public bool CanBeReset()
    {
        if (!canBeReset) return false;
        if (!unlimitedUses && currentUses >= maxResetUses) return false;
        return true;
    }

    public int GetRemainingUses()
    {
        if (unlimitedUses) return 999;
        return maxResetUses - currentUses;
    }

    void OnDrawGizmos()
    {
        if (showResetIndicator)
        {
            Gizmos.color = canBeReset ? indicatorColor : Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }

    void RestoreToOriginal()
{
    // Position
    if (resetPosition)
    {
        transform.position = originalPosition;
        Debug.Log("Restored " + objectName + " to position: " + originalPosition);
    }

    // Rotation
    if (resetRotation)
        transform.rotation = originalRotation;

    // Scale
    if (resetScale)
        transform.localScale = originalScale;

    // Physics
    if (resetPhysics && rb != null)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = originalKinematic;
        rb.useGravity = originalGravity;
    }

    // Active state
    if (resetActiveState)
        gameObject.SetActive(originalActiveState);

    // ── NEW: Notify conduit if this is a conduit ──
    ConduitResettable conduit = GetComponent<ConduitResettable>();
    if (conduit != null)
        conduit.OnConduitReset();
}
}

