using UnityEngine;

public class DoorHandleAnimator : MonoBehaviour
{
    [Header("References")]
    public Animator handleAnimator;

    [Header("Animation Settings")]
    public string pressAnimationName = "DoorHandlePress";

    [Header("Both Handles (Optional)")]
    public Animator otherSideHandle;

    private bool isAnimating = false;

    public void PlayPressAnimation()
    {
        if (!isAnimating)
            StartCoroutine(PlayAnimation());
    }

    System.Collections.IEnumerator PlayAnimation()
    {
        isAnimating = true;

        if (handleAnimator != null)
            handleAnimator.Play(pressAnimationName, 0, 0f);

        if (otherSideHandle != null)
            otherSideHandle.Play(pressAnimationName, 0, 0f);

        // Wait for animation to finish
        yield return new WaitForSeconds(
            GetAnimationLength(pressAnimationName)
        );

        isAnimating = false;
    }

    float GetAnimationLength(string animName)
    {
        if (handleAnimator == null) return 1f;

        RuntimeAnimatorController ac = handleAnimator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animName)
                return clip.length;
        }
        return 1f;
    }
}