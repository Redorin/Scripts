using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterTransition : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName = "Chapter2";
    public SceneTransition sceneTransition;

    private bool hasTriggered = false;

    public void Interact()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminWarning("Sector transfer initiated.");

        if (sceneTransition != null)
            sceneTransition.StartTransition(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }
}