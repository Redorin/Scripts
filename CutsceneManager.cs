using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public PlayableDirector timeline;
    public Image fadeOverlay;
    public PlayerMovement playerMovement;
    public MouseMovement mouseMovement;
    public GameObject cutsceneCamera;
    
    [Header("Controls UI")]
public ControlsUI controlsUI;


    [Header("Timing")]
    public float fadeInDuration = 2f;

    void Start()
    {
        // Disable player control immediately
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (mouseMovement != null)
            mouseMovement.enabled = false;

        // Lock cursor during cutscene
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start fully black
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 1f;
            fadeOverlay.color = c;
        }

        // Stop timeline from auto playing
        if (timeline != null)
            timeline.Stop();

        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
{
    // Step 1: Fade in
    yield return StartCoroutine(FadeIn());

    // Step 2: Play Timeline (handles camera movement AND blink signals)
    if (timeline != null)
    {
        timeline.Play();
        yield return new WaitForSeconds((float)timeline.duration);
    }

    // Step 3: Enable player
    EnablePlayer();
}

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            if (fadeOverlay != null)
            {
                Color c = fadeOverlay.color;
                c.a = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
                fadeOverlay.color = c;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }
    }

    void EnablePlayer()
    {
        // Disable cutscene camera
        if (cutsceneCamera != null)
            cutsceneCamera.SetActive(false);

        // Enable player scripts
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (mouseMovement != null)
            mouseMovement.enabled = true;

        // Sync mouse X rotation
        if (mouseMovement != null)
            mouseMovement.SetXRotation(0f);

        // Admin dialogue
        if (AdminDialogue.Instance != null)
            AdminDialogue.Instance.AdminInfo("Session Initialized.");

            // Show basic controls after opening cutscene
    if (controlsUI != null)
        controlsUI.ShowBasicControls();

        Debug.Log("[CUTSCENE] Complete. Player enabled.");
    }
}