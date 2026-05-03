using UnityEngine;

// Attach to each student model in Chapter 3.
// Blocks reset attempts and notifies Chapter3Manager.
// Students are frozen - no animation, just static models facing wall.

public class StudentObject : MonoBehaviour
{
    [Header("Settings")]
    public bool isBiological = true;    // Biological = cannot be reset

    // Called if player somehow aims Reset Device at student
    // Hook this up by adding a ResettableObject with canBeReset = false
    // OR intercept in ResetItem - see note below

    public void AttemptReset()
    {
        if (Chapter3Manager.Instance != null)
            Chapter3Manager.Instance.OnStudentResetAttempt();

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminWarning("Biological entities outside rollback authority.");
    }
}