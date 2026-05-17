using UnityEngine;
using System.Collections;

// Attach to the parent of all debris pieces (e.g. "CeilingDebris").
// When player resets ANY piece, ALL pieces animate back to ceiling together.
// Ignores player collision during animation to prevent player being thrown.

public class DebrisGroup : MonoBehaviour
{
    [Header("Debris Pieces")]
    public GameObject[] debrisPieces;

    // Add this field at the top with the other headers:
[Header("Barrier")]
public DebrisBarrier debrisBarrier;

    [Header("Animation Settings")]
    public float resetDuration = 1.2f;
    public AnimationCurve resetCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool rotateWhileResetting = true;
    public float spinSpeed = 360f;

    [Header("Shake Before Reset")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.05f;

    [Header("Dialogue")]
    public string[] resetDialogue = {
        "Structural rollback applied.",
        "Path cleared."
    };

    [Header("State")]
    public bool hasBeenReset = false;

    public void ResetAll()
    {
        if (hasBeenReset) return;
        hasBeenReset = true;
        StartCoroutine(AnimatedResetSequence());
    }

    IEnumerator AnimatedResetSequence()
    {
        // Ignore player collision on all pieces during animation
        IgnorePlayerCollision(true);

        // Phase 1: Brief shake
        float shakeElapsed = 0f;
        while (shakeElapsed < shakeDuration)
        {
            foreach (GameObject piece in debrisPieces)
            {
                if (piece == null) continue;
                piece.transform.position += new Vector3(
                    Random.Range(-shakeMagnitude, shakeMagnitude),
                    Random.Range(-shakeMagnitude, shakeMagnitude),
                    Random.Range(-shakeMagnitude, shakeMagnitude)
                );
            }
            shakeElapsed += Time.deltaTime;
            yield return null;
        }

        // Phase 2: Freeze physics, gather start/target positions
        Rigidbody[] rigidbodies = new Rigidbody[debrisPieces.Length];
        Vector3[] startPositions = new Vector3[debrisPieces.Length];
        Quaternion[] startRotations = new Quaternion[debrisPieces.Length];
        Vector3[] targetPositions = new Vector3[debrisPieces.Length];
        Quaternion[] targetRotations = new Quaternion[debrisPieces.Length];

        for (int i = 0; i < debrisPieces.Length; i++)
        {
            if (debrisPieces[i] == null) continue;

            Rigidbody rb = debrisPieces[i].GetComponent<Rigidbody>();
            rigidbodies[i] = rb;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            startPositions[i] = debrisPieces[i].transform.position;
            startRotations[i] = debrisPieces[i].transform.rotation;

            ResettableObject resettable = debrisPieces[i].GetComponent<ResettableObject>();
            if (resettable != null)
            {
                targetPositions[i] = resettable.GetOriginalPosition();
                targetRotations[i] = resettable.GetOriginalRotation();
            }
            else
            {
                targetPositions[i] = startPositions[i];
                targetRotations[i] = startRotations[i];
            }
        }

        // Phase 3: Animate upward
        float elapsed = 0f;
        while (elapsed < resetDuration)
        {
            float t = resetCurve.Evaluate(elapsed / resetDuration);

            for (int i = 0; i < debrisPieces.Length; i++)
            {
                if (debrisPieces[i] == null) continue;

                debrisPieces[i].transform.position = Vector3.Lerp(startPositions[i], targetPositions[i], t);

                if (rotateWhileResetting)
                    debrisPieces[i].transform.Rotate(Vector3.one * spinSpeed * Time.deltaTime);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Phase 4: Snap to final positions
        for (int i = 0; i < debrisPieces.Length; i++)
        {
            if (debrisPieces[i] == null) continue;
            debrisPieces[i].transform.position = targetPositions[i];
            debrisPieces[i].transform.rotation = targetRotations[i];
        }

        // Re-enable player collision after animation
        IgnorePlayerCollision(false);

        // Instability and dialogue
        if (InstabilityManager.Instance != null)
            InstabilityManager.Instance.IncreaseInstability(5f);

        if (AdminDialogue.Instance != null)
        {
            foreach (string line in resetDialogue)
                AdminDialogue.Instance.AdminWarning(line);
        }

        if (debrisBarrier != null)
    debrisBarrier.OnDebrisReset();

        Debug.Log("All debris animated back to ceiling.");
    }

    void IgnorePlayerCollision(bool ignore)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider[] playerColliders = player.GetComponentsInChildren<Collider>();

        foreach (GameObject piece in debrisPieces)
        {
            if (piece == null) continue;
            Collider[] pieceColliders = piece.GetComponentsInChildren<Collider>();

            foreach (Collider pc in playerColliders)
            {
                foreach (Collider dc in pieceColliders)
                {
                    if (pc != null && dc != null)
                        Physics.IgnoreCollision(pc, dc, ignore);
                }
            }
        }
    }
}