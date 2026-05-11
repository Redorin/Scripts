using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChapterTransition : MonoBehaviour
{
    public string nextSceneName = "Chapter2_DistortedSpace";
    public float fadeTime = 2f;
    
    public void Interact()
    {
        if (AdminDialogue.Instance != null)
        {
            AdminDialogue.Instance.AdminWarning("Sector transfer initiated.");
        }
        
        StartCoroutine(TransitionToNextChapter());
    }
    
    IEnumerator TransitionToNextChapter()
    {
        yield return new WaitForSeconds(5f); // Wait 5 seconds
        
        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }
}